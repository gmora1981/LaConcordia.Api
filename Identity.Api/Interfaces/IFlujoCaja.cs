using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IFlujoCaja
    {
        IEnumerable<Flujocaja> GetFlujoCajaInfoAll();

        // El saldo se calcula en el servidor a partir del ultimo registro; el
        // valor de Saldo que venga en el objeto de entrada se ignora.
        void InsertFlujoCaja(Flujocaja New);

        FlujoCajaDTO? GetUltimoRegistro();

        //paginado
        Task<PagedResult<Flujocaja>> GetFlujoCajaPaginados(
            int pagina,
            int pageSize,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string? concepto = null);
    }
}
