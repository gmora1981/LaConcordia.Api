using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenerarCuotaController : Controller
    {
        private readonly IGenerarCuota _generarCuota;

        public GenerarCuotaController(IGenerarCuota generarCuota)
        {
            _generarCuota = generarCuota;
        }

        [HttpGet("GetPendientesPorPeriodo")]
        public IActionResult GetPendientesPorPeriodo(string periodo, string semana)
        {
            try
            {
                var lista = _generarCuota.GetPendientesPorPeriodo(periodo, semana);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al consultar pendientes: " + ex.Message);
            }
        }

        [HttpPost("GenerarCuotaSemanal")]
        public IActionResult GenerarCuotaSemanal([FromQuery] string periodo, [FromQuery] string semana)
        {
            try
            {
                if (string.IsNullOrEmpty(periodo) || string.IsNullOrEmpty(semana))
                    return BadRequest("Periodo y Semana son obligatorios.");

                var resultado = _generarCuota.GenerarCuotaSemanal(periodo, semana);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al generar la cuota semanal: " + ex.Message);
            }
        }

        //paginado
        [HttpGet("GetGenerarCuotaPaginados")]
        public async Task<IActionResult> GetGenerarCuotaPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            string? periodo = null,
            string? semana = null,
            string? cidentidad = null)
        {
            try
            {
                var resultado = await _generarCuota.GetGenerarCuotaPaginados(pagina, pageSize, periodo, semana, cidentidad);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
