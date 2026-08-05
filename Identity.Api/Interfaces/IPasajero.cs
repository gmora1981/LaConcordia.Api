using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IPasajero
    {
        IEnumerable<Pasajero> GetPasajeroInfoAll();
        PasajeroDTO GetPasajeroByCelular(string celular);
        void InsertPasajero(Pasajero New);
        void UpdatePasajero(Pasajero UpdItem);
        void DeletePasajeroByCelular(string celular);

        //paginado
        Task<PagedResult<Pasajero>> GetPasajeroPaginados(
            int pagina,
            int pageSize,
            string? nombres = null,
            string? celular = null);
    }
}
