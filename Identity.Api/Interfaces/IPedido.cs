using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IPedido
    {
        IEnumerable<Pedido> GetPedidoInfoAll();
        void InsertPedido(Pedido New);
        void UpdatePedido(Pedido UpdItem);

        //paginado
        Task<PagedResult<PedidoDTO>> GetPedidoPaginados(
            int pagina,
            int pageSize,
            string? celular = null,
            string? unidad = null,
            string? estado = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null);

        // Chofer actualmente asignado (activo) a una unidad, para autocompletar Conductor/CIConductor.
        ConductorInfoDTO? GetConductorPorUnidad(string unidad);

        // Reutiliza el precio/km de un pedido anterior con exactamente el mismo celular
        // y las mismas coordenadas de origen/destino (igual que hacia el sistema de escritorio).
        PrecioKmDTO? GetPrecioKmHistorico(string celular, decimal origenLat, decimal origenLog, decimal destinoLat, decimal destinoLog);

        // Pedidos que quedaron con el destino "placeholder" (sin definir) y necesitan correccion
        // (pantalla Modificacion de Pedido, equivalente a SE_PEDIDOXCOORDENADAS).
        List<PedidoDTO> GetPedidosConDestinoPendiente();

        // Guarda la direccion resuelta de unas coordenadas (requiere que el pasajero ya exista).
        void GuardarDireccion(string celular, decimal lat, decimal lng, string? calle);

        // Dashboard "Total de Ingresos de Carreras Asignadas": cantidad de pedidos por usuario.
        List<PedidosPorUsuarioDTO> GetCantidadPedidosPorUsuario(DateTime desde, DateTime hasta);

        // "Reporte de Solicitud de Carrera" por usuario/operadora y rango de fechas.
        List<string> GetUsuariosDisponibles();
        List<PedidoOperadoraDTO> GetPedidosPorOperadora(string? usuario, DateTime desde, DateTime hasta);
        byte[] ExportarReporteSolicitudCarreraPdf(string? usuario, DateTime desde, DateTime hasta, string usuarioLogueado);
    }
}
