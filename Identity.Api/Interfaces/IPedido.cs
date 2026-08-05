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
    }
}
