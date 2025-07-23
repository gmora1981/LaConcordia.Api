using Identity.Api.Interfaces;
using Identity.Api.Paginado;
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
    public class EmpresaController : Controller
    {
        private readonly IEmpresa _empresa;

        public EmpresaController(IEmpresa empresa)
        {
            _empresa = empresa;
        }

        [HttpGet("GetEmpresaInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _empresa.GetEmpresaInfoAll();

            return Ok(lista);
        }
        [HttpGet("GetEmpresaByRuc/{ruc}")]
        public IActionResult GetByRuc(string ruc)
        {
            var item = _empresa.GetEmpresaByRuc(ruc);
            if (item == null)
                return NotFound("Empresa no encontrada.");
            return Ok(item);
        }
        [HttpPost("InsertEmpresa")]
        public IActionResult Create([FromBody] Empresa nueva)
        {
            try
            {
                _empresa.InsertEmpresa(nueva);
                return Ok("Empresa creada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }
        [HttpPut("UpdateEmpresa")]
        public IActionResult Update([FromBody] Empresa actualizada)
        {
            try
            {
                _empresa.UpdateEmpresa(actualizada);
                return Ok("Empresa actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }
        [HttpDelete("DeleteEmpresaByRuc/{ruc}")]
        public IActionResult Delete(string ruc)
        {
            try
            {
                _empresa.DeleteEmpresaByRuc(ruc);
                return Ok("Empresa eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }

        }
        //paginado  
        [HttpGet("GetEmpresasPaginados")]
        public async Task<IActionResult> GetEmpresasPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            string? ruc = null,
            string? razonsocial = null,
            string? telefono = null,
            string? estado = null)
        {
            try
            {
                var resultado = await _empresa.GetEmpresasPaginados(pagina, pageSize, ruc, razonsocial, telefono, estado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        //exportar
        [HttpGet("exportarPDF")]
        public IActionResult ExportarEmpresasPdf(string? filtro = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var datos = _empresa.ObtenerEmpresasFiltradas(filtro);

            if (datos == null || !datos.Any())
                return NotFound("No hay datos para exportar.");

            var pdfBytes = EmpresaPdfGenerator.GenerarPdf(datos);

            return File(pdfBytes, "application/pdf", "EmpresasListado.pdf");
        }
    }
}
