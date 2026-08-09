using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BalanceController : Controller
    {
        private readonly IBalance _balance;

        public BalanceController(IBalance balance)
        {
            _balance = balance;
        }

        [HttpGet("GetBalance")]
        public IActionResult GetBalance(DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                var resultado = _balance.GetBalance(fechaDesde, fechaHasta);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al generar el balance: " + ex.Message);
            }
        }

        [HttpGet("ExportarPdf")]
        public IActionResult ExportarPdf(DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                var usuario = User.Identity?.Name ?? "";
                var pdfBytes = _balance.ExportarBalancePdf(fechaDesde, fechaHasta, usuario);
                return File(pdfBytes, "application/pdf", "BalanceInicial.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al exportar el balance: " + ex.Message);
            }
        }
    }
}
