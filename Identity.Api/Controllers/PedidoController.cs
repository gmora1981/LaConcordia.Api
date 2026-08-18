using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Model;
using Identity.Api.Paginado;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidoController : Controller
    {
        private readonly IPedido _pedido;
        private readonly UserManager<ApplicationUser> _userManager;

        public PedidoController(IPedido pedido, UserManager<ApplicationUser> userManager)
        {
            _pedido = pedido;
            _userManager = userManager;
        }

        // Resuelve el usuario logueado (JWT) a su Cedula (ApplicationUser.Cedula), para las
        // rutas de la app del conductor. Devuelve null si el usuario no tiene Cedula asignada.
        private async Task<string?> ObtenerCedulaLogueado()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return null;

            var user = await _userManager.FindByNameAsync(username);
            return user?.Cedula;
        }

        [HttpGet("GetPedidoInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _pedido.GetPedidoInfoAll();

            return Ok(lista);
        }

        [HttpPost("InsertPedido")]
        public IActionResult Create([FromBody] Pedido nuevo)
        {
            try
            {
                _pedido.InsertPedido(nuevo);
                return Ok("Pedido creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdatePedido")]
        public IActionResult Update([FromBody] Pedido actualizado)
        {
            try
            {
                _pedido.UpdatePedido(actualizado);
                return Ok("Pedido actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        //paginado
        [HttpGet("GetPedidoPaginados")]
        public async Task<IActionResult> GetPedidoPaginados(
            int pagina = 1,
            int pageSize = PaginadorHelper.NumeroDeDatosPorPagina,
            string? celular = null,
            string? unidad = null,
            string? estado = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            try
            {
                var resultado = await _pedido.GetPedidoPaginados(pagina, pageSize, celular, unidad, estado, fechaDesde, fechaHasta);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetConductorPorUnidad/{unidad}")]
        public IActionResult GetConductorPorUnidad(string unidad)
        {
            var item = _pedido.GetConductorPorUnidad(unidad);
            if (item == null)
                return NotFound("No hay un conductor activo asignado a esa unidad.");
            return Ok(item);
        }

        [HttpGet("GetPrecioKmHistorico")]
        public IActionResult GetPrecioKmHistorico(string celular, decimal origenLat, decimal origenLog, decimal destinoLat, decimal destinoLog)
        {
            var item = _pedido.GetPrecioKmHistorico(celular, origenLat, origenLog, destinoLat, destinoLog);
            return Ok(item);
        }

        [HttpGet("GetPedidosConDestinoPendiente")]
        public IActionResult GetPedidosConDestinoPendiente()
        {
            return Ok(_pedido.GetPedidosConDestinoPendiente());
        }

        // Guarda la direccion resuelta de unas coordenadas para que Orden de Pago pueda
        // mostrar el Punto de Partida/Final. No falla si el pasajero aun no existe.
        [HttpPost("GuardarDireccion")]
        public IActionResult GuardarDireccion([FromBody] GuardarDireccionRequestDTO request)
        {
            try
            {
                _pedido.GuardarDireccion(request.Celular, request.Lat, request.Lng, request.Calle);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error al guardar la dirección: " + ex.Message);
            }
        }

        // Dashboard "Total de Ingresos de Carreras Asignadas": cantidad de pedidos por usuario.
        [HttpGet("GetCantidadPedidosPorUsuario")]
        public IActionResult GetCantidadPedidosPorUsuario(DateTime desde, DateTime hasta)
        {
            try
            {
                return Ok(_pedido.GetCantidadPedidosPorUsuario(desde, hasta));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Dashboard "Top 10 de las Unidades con Mas Carreras".
        [HttpGet("GetTopUnidadesConMasCarreras")]
        public IActionResult GetTopUnidadesConMasCarreras(DateTime desde, DateTime hasta)
        {
            try
            {
                return Ok(_pedido.GetTopUnidadesConMasCarreras(desde, hasta));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // "Reporte de Solicitud de Carrera" por usuario/operadora y rango de fechas.
        [HttpGet("GetUsuariosDisponibles")]
        public IActionResult GetUsuariosDisponibles()
        {
            return Ok(_pedido.GetUsuariosDisponibles());
        }

        [HttpGet("GetUnidadesConPedidos")]
        public IActionResult GetUnidadesConPedidos()
        {
            return Ok(_pedido.GetUnidadesConPedidos());
        }

        [HttpGet("GetPedidosPorOperadora")]
        public IActionResult GetPedidosPorOperadora(string? usuario, DateTime desde, DateTime hasta, string? unidad = null)
        {
            try
            {
                return Ok(_pedido.GetPedidosPorOperadora(usuario, desde, hasta, unidad));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("ExportarReporteSolicitudCarreraPdf")]
        public IActionResult ExportarReporteSolicitudCarreraPdf(string? usuario, DateTime desde, DateTime hasta, string? unidad = null)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                var usuarioLogueado = User.Identity?.Name ?? "desconocido";
                var pdfBytes = _pedido.ExportarReporteSolicitudCarreraPdf(usuario, desde, hasta, unidad, usuarioLogueado);
                return File(pdfBytes, "application/pdf", "ReporteSolicitudCarrera.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al exportar el reporte: " + ex.Message);
            }
        }

        // ===== App del conductor (Taxista) =====

        [HttpGet("GetMiInfoConductor")]
        [Authorize(Roles = "Taxista")]
        public async Task<IActionResult> GetMiInfoConductor()
        {
            var cedula = await ObtenerCedulaLogueado();
            if (string.IsNullOrEmpty(cedula))
                return BadRequest("Esta cuenta no tiene una cédula de conductor vinculada.");

            var info = _pedido.GetInfoConductorPorCedula(cedula);
            if (info == null)
                return NotFound("No se encontró una ficha personal activa para esta cédula.");

            return Ok(info);
        }

        [HttpGet("GetCarrerasAsignadas")]
        [Authorize(Roles = "Taxista")]
        public async Task<IActionResult> GetCarrerasAsignadas(string? estado = null)
        {
            var cedula = await ObtenerCedulaLogueado();
            if (string.IsNullOrEmpty(cedula))
                return BadRequest("Esta cuenta no tiene una cédula de conductor vinculada.");

            var info = _pedido.GetInfoConductorPorCedula(cedula);
            if (info?.Unidad == null)
                return NotFound("No se encontró una unidad asignada para esta cédula.");

            return Ok(_pedido.GetCarrerasAsignadas(info.Unidad, estado));
        }

        [HttpPost("TomarCarrera")]
        [Authorize(Roles = "Taxista")]
        public async Task<IActionResult> TomarCarrera([FromBody] TomarCarreraRequestDTO request)
        {
            try
            {
                var cedula = await ObtenerCedulaLogueado();
                if (string.IsNullOrEmpty(cedula))
                    return BadRequest("Esta cuenta no tiene una cédula de conductor vinculada.");

                _pedido.TomarCarrera(request, cedula);
                return Ok("Carrera tomada correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al tomar la carrera: " + ex.Message);
            }
        }

        [HttpPost("FinalizarCarrera")]
        [Authorize(Roles = "Taxista")]
        public IActionResult FinalizarCarrera([FromBody] FinalizarCarreraRequestDTO request)
        {
            try
            {
                _pedido.FinalizarCarrera(request);
                return Ok("Carrera finalizada correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al finalizar la carrera: " + ex.Message);
            }
        }

        [HttpGet("GetGananciasConductor")]
        [Authorize(Roles = "Taxista")]
        public async Task<IActionResult> GetGananciasConductor(DateTime desde, DateTime hasta)
        {
            var cedula = await ObtenerCedulaLogueado();
            if (string.IsNullOrEmpty(cedula))
                return BadRequest("Esta cuenta no tiene una cédula de conductor vinculada.");

            return Ok(_pedido.GetGananciasConductor(cedula, desde, hasta));
        }

        [HttpGet("GetCalificacionCarrera")]
        public IActionResult GetCalificacionCarrera([FromQuery] PedidoIdentificadorDTO id)
        {
            return Ok(_pedido.GetCalificacionCarrera(id));
        }

        // El despachador/admin califica el viaje desde la pantalla de Pedido.
        [HttpPost("CalificarCarrera")]
        public IActionResult CalificarCarrera([FromBody] CalificarCarreraRequestDTO request)
        {
            try
            {
                _pedido.CalificarCarrera(request);
                return Ok("Calificación guardada correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al guardar la calificación: " + ex.Message);
            }
        }
    }
}
