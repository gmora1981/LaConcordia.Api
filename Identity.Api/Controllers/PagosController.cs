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
    public class PagosController : Controller
    {
        private readonly IPagos _pagos;

        public PagosController(IPagos pagos)
        {
            _pagos = pagos;
        }

        [HttpGet("GetDeudaSocio/{cedula}")]
        public IActionResult GetDeudaSocio(string cedula)
        {
            var deuda = _pagos.GetDeudaSocio(cedula);
            if (deuda == null)
                return NotFound("Socio no encontrado.");
            return Ok(deuda);
        }

        [HttpGet("ExisteComprobante")]
        public IActionResult ExisteComprobante(string banco, string numComprobante)
        {
            return Ok(_pagos.ExisteComprobante(banco, numComprobante));
        }

        [HttpPost("PagarCuota")]
        public IActionResult PagarCuota([FromBody] PagoCuotaRequestDTO request)
        {
            try
            {
                var usuario = User.Identity?.Name ?? "desconocido";
                _pagos.PagarCuota(request, usuario);
                return Ok("Pago registrado con éxito.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al registrar el pago: " + ex.Message);
            }
        }

        [HttpPost("PagarUbm")]
        public IActionResult PagarUbm([FromBody] PagoUbmRequestDTO request)
        {
            try
            {
                var usuario = User.Identity?.Name ?? "desconocido";
                _pagos.PagarUbm(request, usuario);
                return Ok("Pago registrado con éxito.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al registrar el pago: " + ex.Message);
            }
        }
    }
}
