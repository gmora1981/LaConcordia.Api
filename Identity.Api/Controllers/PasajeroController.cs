using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PasajeroController : Controller
    {
        private readonly IPasajero _pasajero;

        public PasajeroController(IPasajero pasajero)
        {
            _pasajero = pasajero;
        }

        [HttpGet("GetPasajeroInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _pasajero.GetPasajeroInfoAll();

            return Ok(lista);
        }

        [HttpGet("GetPasajeroByCelular/{celular}")]
        public IActionResult GetByCelular(string celular)
        {
            var item = _pasajero.GetPasajeroByCelular(celular);
            if (item == null)
                return NotFound("Pasajero no encontrado.");
            return Ok(item);
        }

        [HttpPost("InsertPasajero")]
        public IActionResult Create([FromBody] Pasajero nuevo)
        {
            try
            {
                _pasajero.InsertPasajero(nuevo);
                return Ok("Pasajero creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdatePasajero")]
        public IActionResult Update([FromBody] Pasajero actualizado)
        {
            try
            {
                _pasajero.UpdatePasajero(actualizado);
                return Ok("Pasajero actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeletePasajeroByCelular/{celular}")]
        public IActionResult Delete(string celular)
        {
            try
            {
                _pasajero.DeletePasajeroByCelular(celular);
                return Ok("Pasajero eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }
        }

        //paginado
        [HttpGet("GetPasajeroPaginados")]
        public async Task<IActionResult> GetPasajeroPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            string? nombres = null,
            string? celular = null)
        {
            try
            {
                var resultado = await _pasajero.GetPasajeroPaginados(pagina, pageSize, nombres, celular);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
