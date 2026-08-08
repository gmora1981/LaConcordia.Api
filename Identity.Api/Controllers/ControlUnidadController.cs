using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Reporteria;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Infrastructure;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ControlUnidadController : Controller
    {
        private readonly IControlUnidad _controlUnidad;

        public ControlUnidadController(IControlUnidad controlUnidad)
        {
            _controlUnidad = controlUnidad;
        }

        [HttpGet("GetFichaPersonalPorServicio/{estadoServicio}")]
        public IActionResult GetFichaPersonalPorServicio(string estadoServicio)
        {
            return Ok(_controlUnidad.GetFichaPersonalPorServicio(estadoServicio));
        }

        [HttpPost("MoverUnidad")]
        public IActionResult MoverUnidad([FromBody] MoverUnidadRequestDTO request)
        {
            try
            {
                var monitora = User.Identity?.Name ?? "desconocido";
                _controlUnidad.MoverUnidad(request, monitora);
                return Ok("Movimiento registrado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al registrar el movimiento: " + ex.Message);
            }
        }

        //exportar
        [HttpGet("exportarPDF")]
        public IActionResult ExportarPdf(string? turno = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var fueraDeServicio = _controlUnidad.GetFichaPersonalPorServicio("p");
            var enServicio = _controlUnidad.GetFichaPersonalPorServicio("a");
            var monitora = User.Identity?.Name ?? "desconocido";

            var pdfBytes = ControlUnidadPdfGenerator.GenerarPdf(fueraDeServicio, enServicio, turno, monitora);

            return File(pdfBytes, "application/pdf", "ControlUnidades.pdf");
        }
    }
}
