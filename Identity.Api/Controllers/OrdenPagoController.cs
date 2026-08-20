using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdenPagoController : Controller
    {
        private readonly IOrdenPago _ordenPago;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdenPagoController(IOrdenPago ordenPago, UserManager<ApplicationUser> userManager)
        {
            _ordenPago = ordenPago;
            _userManager = userManager;
        }

        // Resuelve el usuario logueado (JWT) a su Ruc (ApplicationUser.Ruc), para las rutas
        // del portal de empresas. Mismo patron que PedidoController.ObtenerCedulaLogueado.
        private async Task<string?> ObtenerRucLogueado()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return null;

            var user = await _userManager.FindByNameAsync(username);
            return user?.Ruc;
        }

        [HttpGet("GetMiRuc")]
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> GetMiRuc()
        {
            var ruc = await ObtenerRucLogueado();
            if (string.IsNullOrEmpty(ruc))
                return BadRequest("Esta cuenta no tiene una empresa vinculada.");

            return Ok(ruc);
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

        // Si el usuario tiene rol Empresa, solo puede pedir su propio Ruc (evita que edite la
        // URL/parametro y vea la facturacion de otra empresa). Los demas roles (Admin) no
        // tienen esta restriccion.
        private async Task<bool> RucPermitidoParaUsuarioActual(string ruc)
        {
            if (!User.IsInRole("Empresa")) return true;

            var miRuc = await ObtenerRucLogueado();
            return !string.IsNullOrEmpty(miRuc) && miRuc == ruc;
        }

        [HttpGet("GetOrdenPagoPorEmpresa/{ruc}")]
        public async Task<IActionResult> GetOrdenPagoPorEmpresa(string ruc, DateTime? hasta = null)
        {
            if (!await RucPermitidoParaUsuarioActual(ruc))
                return Forbid();

            return Ok(_ordenPago.GetOrdenPagoPorEmpresa(ruc, hasta));
        }

        [HttpGet("ExportarFacturacionPdf")]
        public async Task<IActionResult> ExportarFacturacionPdf(string ruc, string razonSocial, DateTime? hasta = null)
        {
            if (!await RucPermitidoParaUsuarioActual(ruc))
                return Forbid();

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

        // Igual que GetResumenVouchers pero acotado al Ruc del usuario Empresa logueado (no se
        // acepta el Ruc por parametro para que una empresa no pueda ver datos de otra).
        [HttpGet("GetResumenVouchersEmpresa")]
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> GetResumenVouchersEmpresa(DateTime desde, DateTime hasta)
        {
            var ruc = await ObtenerRucLogueado();
            if (string.IsNullOrEmpty(ruc))
                return BadRequest("Esta cuenta no tiene una empresa vinculada.");

            return Ok(_ordenPago.GetResumenVouchers(desde, hasta, ruc));
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
