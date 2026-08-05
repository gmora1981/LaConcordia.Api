using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdenPagoController : Controller
    {
        private readonly IOrdenPago _ordenPago;

        public OrdenPagoController(IOrdenPago ordenPago)
        {
            _ordenPago = ordenPago;
        }

        [HttpGet("GetPedidosPendientesVoucher")]
        public IActionResult GetPedidosPendientesVoucher(string? ruc = null)
        {
            return Ok(_ordenPago.GetPedidosPendientesVoucher(ruc));
        }

        [HttpGet("GetDireccionTexto")]
        public IActionResult GetDireccionTexto(string celular, decimal lat, decimal lng)
        {
            return Ok(_ordenPago.GetDireccionTexto(celular, lat, lng));
        }

        [HttpGet("GetSaldoCajaActual")]
        public IActionResult GetSaldoCajaActual()
        {
            return Ok(_ordenPago.GetSaldoCajaActual());
        }

        [HttpPost("GenerarOrdenPago")]
        public IActionResult GenerarOrdenPago([FromBody] GenerarOrdenPagoRequestDTO request)
        {
            try
            {
                var usuario = User.Identity?.Name ?? "desconocido";
                var resultado = _ordenPago.GenerarOrdenPago(request, usuario);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al generar la orden de pago: " + ex.Message);
            }
        }
    }
}
