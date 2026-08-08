using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class PedidoServices : IPedido
    {
        private PedidoRepository _pedido = new PedidoRepository();

        public IEnumerable<Pedido> GetPedidoInfoAll()
        {
            return _pedido.GetPedidoInfoAll();
        }

        public void InsertPedido(Pedido New)
        {
            _pedido.InsertPedido(New);
        }

        public void UpdatePedido(Pedido UpdItem)
        {
            _pedido.UpdatePedido(UpdItem);
        }

        //paginado
        public async Task<PagedResult<PedidoDTO>> GetPedidoPaginados(
            int pagina,
            int pageSize,
            string? celular = null,
            string? unidad = null,
            string? estado = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            return await _pedido.GetPedidoPaginados(pagina, pageSize, celular, unidad, estado, fechaDesde, fechaHasta);
        }

        public ConductorInfoDTO? GetConductorPorUnidad(string unidad)
        {
            return _pedido.GetConductorPorUnidad(unidad);
        }

        public PrecioKmDTO? GetPrecioKmHistorico(string celular, decimal origenLat, decimal origenLog, decimal destinoLat, decimal destinoLog)
        {
            return _pedido.GetPrecioKmHistorico(celular, origenLat, origenLog, destinoLat, destinoLog);
        }

        public List<PedidoDTO> GetPedidosConDestinoPendiente()
        {
            return _pedido.GetPedidosConDestinoPendiente();
        }

        public void GuardarDireccion(string celular, decimal lat, decimal lng, string? calle)
        {
            _pedido.GuardarDireccion(celular, lat, lng, calle);
        }
    }
}
