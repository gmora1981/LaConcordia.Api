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

        [HttpGet("GetOrdenPagoPorEmpresa/{ruc}")]
        public IActionResult GetOrdenPagoPorEmpresa(string ruc, DateTime? hasta = null)
        {
            return Ok(_ordenPago.GetOrdenPagoPorEmpresa(ruc, hasta));
        }

        [HttpGet("ExportarFacturacionPdf")]
        public IActionResult ExportarFacturacionPdf(string ruc, string razonSocial, DateTime? hasta = null)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                var usuario = User.Identity?.Name ?? "desconocido";
                var pdfBytes = _ordenPago.ExportarFacturacionPdf(ruc, razonSocial, hasta, usuario);
                return File(pdfBytes, "application/pdf", "Facturacion.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al exportar la facturación: " + ex.Message);
            }
        }

        [HttpGet("GetVouchersPendientesPorUnidad")]
        public IActionResult GetVouchersPendientesPorUnidad(string? unidad, DateTime desde, DateTime hasta)
        {
            try
            {
                return Ok(_ordenPago.GetVouchersPendientesPorUnidad(unidad, desde, hasta));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("ExportarReporteVoucherPagarPdf")]
        public IActionResult ExportarReporteVoucherPagarPdf(string? unidad, DateTime desde, DateTime hasta)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                var usuario = User.Identity?.Name ?? "desconocido";
                var pdfBytes = _ordenPago.ExportarReporteVoucherPagarPdf(unidad, desde, hasta, usuario);
                return File(pdfBytes, "application/pdf", "ReporteVoucherPorPagar.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al exportar el reporte: " + ex.Message);
            }
        }

        [HttpGet("GetResumenVouchers")]
        public IActionResult GetResumenVouchers(DateTime desde, DateTime hasta)
        {
            return Ok(_ordenPago.GetResumenVouchers(desde, hasta));
        }

        // "Modificar Datos": corrige Precio/Recorrido/Empleado del pedido sin generar voucher.
        [HttpPut("ActualizarDatosPedido")]
        public IActionResult ActualizarDatosPedido([FromBody] ActualizarDatosPedidoRequestDTO request)
        {
            try
            {
                var usuario = User.Identity?.Name ?? "desconocido";
                _ordenPago.ActualizarDatosPedido(request, usuario);
                return Ok("Datos del pedido actualizados correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar los datos: " + ex.Message);
            }
        }
    }
}
