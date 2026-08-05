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
    }
}
