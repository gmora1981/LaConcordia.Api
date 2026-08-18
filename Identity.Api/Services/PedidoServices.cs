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

        public List<PedidosPorUsuarioDTO> GetCantidadPedidosPorUsuario(DateTime desde, DateTime hasta)
        {
            return _pedido.GetCantidadPedidosPorUsuario(desde, hasta);
        }

        public List<PedidosPorUnidadDTO> GetTopUnidadesConMasCarreras(DateTime desde, DateTime hasta)
        {
            return _pedido.GetTopUnidadesConMasCarreras(desde, hasta);
        }

        public List<string> GetUsuariosDisponibles()
        {
            return _pedido.GetUsuariosDisponibles();
        }

        public List<string> GetUnidadesConPedidos()
        {
            return _pedido.GetUnidadesConPedidos();
        }

        public List<PedidoOperadoraDTO> GetPedidosPorOperadora(string? usuario, DateTime desde, DateTime hasta, string? unidad = null)
        {
            return _pedido.GetPedidosPorOperadora(usuario, desde, hasta, unidad);
        }

        public byte[] ExportarReporteSolicitudCarreraPdf(string? usuario, DateTime desde, DateTime hasta, string? unidad, string usuarioLogueado)
        {
            return _pedido.ExportarReporteSolicitudCarreraPdf(usuario, desde, hasta, unidad, usuarioLogueado);
        }

        public InfoConductorDTO? GetInfoConductorPorCedula(string cedula)
        {
            return _pedido.GetInfoConductorPorCedula(cedula);
        }

        public List<PedidoDTO> GetCarrerasAsignadas(string unidad, string? estado = null)
        {
            return _pedido.GetCarrerasAsignadas(unidad, estado);
        }

        public void TomarCarrera(TomarCarreraRequestDTO request, string cedulaConductor)
        {
            _pedido.TomarCarrera(request, cedulaConductor);
        }

        public void FinalizarCarrera(FinalizarCarreraRequestDTO request)
        {
            _pedido.FinalizarCarrera(request);
        }

        public GananciasConductorDTO GetGananciasConductor(string cedula, DateTime desde, DateTime hasta)
        {
            return _pedido.GetGananciasConductor(cedula, desde, hasta);
        }

        public void CalificarCarrera(CalificarCarreraRequestDTO request)
        {
            _pedido.CalificarCarrera(request);
        }

        public CalificarCarreraRequestDTO? GetCalificacionCarrera(PedidoIdentificadorDTO id)
        {
            return _pedido.GetCalificacionCarrera(id);
        }
    }
}
