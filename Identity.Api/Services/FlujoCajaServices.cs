using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class FlujoCajaServices : IFlujoCaja
    {
        private FlujoCajaRepository _flujoCaja = new FlujoCajaRepository();

        public IEnumerable<Flujocaja> GetFlujoCajaInfoAll()
        {
            return _flujoCaja.GetFlujoCajaInfoAll();
        }

        public void InsertFlujoCaja(Flujocaja New)
        {
            _flujoCaja.InsertFlujoCaja(New);
        }

        public FlujoCajaDTO? GetUltimoRegistro()
        {
            return _flujoCaja.GetUltimoRegistro();
        }

        //paginado
        public async Task<PagedResult<Flujocaja>> GetFlujoCajaPaginados(
            int pagina,
            int pageSize,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string? concepto = null)
        {
            return await _flujoCaja.GetFlujoCajaPaginados(pagina, pageSize, fechaDesde, fechaHasta, concepto);
        }
    }
}
