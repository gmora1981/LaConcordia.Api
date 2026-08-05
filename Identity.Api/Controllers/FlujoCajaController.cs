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
    public class FlujoCajaController : Controller
    {
        private readonly IFlujoCaja _flujoCaja;

        public FlujoCajaController(IFlujoCaja flujoCaja)
        {
            _flujoCaja = flujoCaja;
        }

        [HttpGet("GetFlujoCajaInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _flujoCaja.GetFlujoCajaInfoAll();

            return Ok(lista);
        }

        [HttpGet("GetUltimoRegistro")]
        public IActionResult GetUltimoRegistro()
        {
            var item = _flujoCaja.GetUltimoRegistro();
            return Ok(item);
        }

        [HttpPost("InsertFlujoCaja")]
        public IActionResult Create([FromBody] Flujocaja nuevo)
        {
            try
            {
                _flujoCaja.InsertFlujoCaja(nuevo);
                return Ok("Movimiento de caja registrado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        //paginado
        [HttpGet("GetFlujoCajaPaginados")]
        public async Task<IActionResult> GetFlujoCajaPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string? concepto = null)
        {
            try
            {
                var resultado = await _flujoCaja.GetFlujoCajaPaginados(pagina, pageSize, fechaDesde, fechaHasta, concepto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
