using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Model.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UnidadController : Controller
    {

        private readonly IUnidad _unidadRepository;

        public UnidadController(IUnidad unidadRepository)
        {
            _unidadRepository = unidadRepository;
        }

        [HttpGet("GetUnidadInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _unidadRepository.GetUnidadInfoAll();
            return Ok(lista);
        }

        [HttpGet("GetUnidadById/{idUnidad}")]
        public IActionResult GetById(string idUnidad)
        {
            var item = _unidadRepository.GetUnidadById(idUnidad);
            if (item == null)
                return NotFound("Unidad no encontrada.");
            return Ok(item);
        }

        [HttpPost("InsertUnidad")]
        public IActionResult Create([FromBody] UnidadDTO nueva)
        {
            if (nueva == null)
                return BadRequest("La unidad no puede ser nula.");

            try
            {
                _unidadRepository.InsertUnidad(nueva);
                return Ok(new { mensaje = "Unidad creada correctamente." });
            }
            catch (Exception ex)
            {
                // Puedes registrar el error si usas logging
                return StatusCode(500, new { error = "Error al crear la unidad.", detalle = ex.Message });
            }
        }


        [HttpPut("UpdateUnidad")]
        public IActionResult Update([FromBody] Unidad actualizada)
        {
            try
            {
                _unidadRepository.UpdateUnidad(actualizada);
                return Ok("Unidad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteUnidad/{idUnidad}")]
        public IActionResult DeleteById(string idUnidad)
        {
            try
            {
                _unidadRepository.DeleteUnidadById(idUnidad);
                return Ok("Parentesco eliminado por ID correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar por ID: " + ex.Message);
            }
        }

        // Paginado
        [HttpGet("GetUnidadPaginados")]
        public async Task<IActionResult> GetUnidadPaginados(
            int pagina = 1,
            int pageSize = 10,
            string? Placa = null,
            string? Idpropietario = null,
            string? Unidad1 = null,
            string? Propietario = null,
            string? Estado = null)
        {
            try
            {
                var result = await _unidadRepository.GetUnidadPaginados(pagina, pageSize, Placa, Idpropietario, Unidad1, Propietario, Estado);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener unidades paginadas: " + ex.Message);
            }
        }

    }
}
