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
        public IActionResult ExportarPdf(string? turno = null, DateTime? fecha = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var movimientos = _controlUnidad.GetMovimientos(fecha ?? DateTime.Today, turno);
            var usuario = User.Identity?.Name ?? "desconocido";

            var pdfBytes = ControlUnidadPdfGenerator.GenerarPdf(movimientos, usuario);

            return File(pdfBytes, "application/pdf", "ControlUnidades.pdf");
        }

        // "Reporte de Ingreso y Salida" por operadora y rango de fechas.
        [HttpGet("GetMonitorasDisponibles")]
        public IActionResult GetMonitorasDisponibles()
        {
            return Ok(_controlUnidad.GetMonitorasDisponibles());
        }

        [HttpGet("GetMovimientosPorRango")]
        public IActionResult GetMovimientosPorRango(DateTime desde, DateTime hasta, string? monitora = null)
        {
            try
            {
                return Ok(_controlUnidad.GetMovimientosPorRango(desde, hasta, monitora));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("ExportarReporteIngresoSalidaPdf")]
        public IActionResult ExportarReporteIngresoSalidaPdf(DateTime desde, DateTime hasta, string? monitora = null)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var usuario = User.Identity?.Name ?? "desconocido";
                var pdfBytes = _controlUnidad.ExportarReporteIngresoSalidaPdf(desde, hasta, monitora, usuario);
                return File(pdfBytes, "application/pdf", "ReporteIngresoYSalida.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al exportar el reporte: " + ex.Message);
            }
        }
    }
}
