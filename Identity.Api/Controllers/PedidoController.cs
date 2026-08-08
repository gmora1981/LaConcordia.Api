using Identity.Api.DTO;
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
    public class PedidoController : Controller
    {
        private readonly IPedido _pedido;

        public PedidoController(IPedido pedido)
        {
            _pedido = pedido;
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
    }
}
