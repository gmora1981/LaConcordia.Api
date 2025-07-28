using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Reporteria;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;
using QuestPDF.Infrastructure;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DuenopuestoController : ControllerBase
    {
        private readonly IDuenopuesto _duenoPuesto;

        public DuenopuestoController(IDuenopuesto duenopuesto)
        {
            _duenoPuesto = duenopuesto;
        }


        [HttpGet("DuenopuestoInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _duenoPuesto.GetDuenopuestoInfoAll();

            return Ok(lista);
        }

        [HttpGet("GetDuenopuestoById/{cedula}")]
        public IActionResult GetById(string cedula)
        {
            var item = _duenoPuesto.GetDuenopuestoById(cedula);
            if (item == null)
                return NotFound("Dueño de puesto no encontrado.");
            return Ok(item);
        }

        [HttpPost("InsertDuenopuesto")]
        public IActionResult Create([FromBody] DuenopuestoDTO nueva)
        {
            try
            {
                _duenoPuesto.InsertDuenopuesto(nueva);
                return Ok("Dueño de puesto creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateDuenopuesto")]
        public IActionResult Update([FromBody] DuenopuestoDTO actualizada)
        {
            try
            {
                _duenoPuesto.UpdateDuenopuesto(actualizada);
                return Ok("Dueño de puesto actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteDuenopuestoById/{cedula}")]
        public IActionResult DeleteById(string cedula)
        {
            try
            {
                _duenoPuesto.DeletePDuenopuestoById(cedula);
                return Ok("Dueño de puesto eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }

        }

        [HttpGet("GetDuenopuestosPaginados")]
        public async Task<IActionResult> GetDuenopuestosPaginados(
            int pagina = 1,
            int pageSize = 10,
            string? cedula = null,
            string? nombre = null,
            string? apellidos = null,
            string? estado = null)
        {
            var result = await _duenoPuesto.GetDuenopuestosPaginados(pagina, pageSize, cedula, nombre, apellidos, estado);
            return Ok(result);

        }

        //exportar
        [HttpGet("exportarPDF")]
        public IActionResult ExportarEmpresasPdf(string? filtro = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var datos = _duenoPuesto.ObtenerDuenoPuestoFiltradas(filtro);

            if (datos == null || !datos.Any())
                return NotFound("No hay datos para exportar.");

            var pdfBytes = DuenoDePuestoPdfGenerator.GenerarPdf(datos);

            return File(pdfBytes, "application/pdf", "DueñoDePuestoListado.pdf");
        }
    }
}
