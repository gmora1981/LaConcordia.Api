using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class PasajeroServices : IPasajero
    {
        private PasajeroRepository _pasajero = new PasajeroRepository();

        public IEnumerable<Pasajero> GetPasajeroInfoAll()
        {
            return _pasajero.GetPasajeroInfoAll();
        }

        public PasajeroDTO GetPasajeroByCelular(string celular)
        {
            return _pasajero.GetPasajeroByCelular(celular);
        }

        public void InsertPasajero(Pasajero New)
        {
            _pasajero.InsertPasajero(New);
        }

        public void UpdatePasajero(Pasajero UpdItem)
        {
            _pasajero.UpdatePasajero(UpdItem);
        }

        public void DeletePasajeroByCelular(string celular)
        {
            _pasajero.DeletePasajeroByCelular(celular);
        }

        //paginado
        public async Task<PagedResult<Pasajero>> GetPasajeroPaginados(int pagina, int pageSize, string? nombres = null, string? celular = null)
        {
            return await _pasajero.GetPasajeroPaginados(pagina, pageSize, nombres, celular);
        }
    }
}
