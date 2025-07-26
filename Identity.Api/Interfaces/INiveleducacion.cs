using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface INiveleducacion
    {
        IEnumerable<Niveleducacion> GetNiveleducacionInfoAll();
        NiveleducacionDTO GetNiveleducacionById(int idNiveleducacion);
        void InsertNiveleducacion(Niveleducacion nueva);
        void UpdateNiveleducacion(Niveleducacion actualizada);
        void DeleteNiveleducacionById(int idNiveleducacion);

        Task<PagedResult<Niveleducacion>> GetNiveleducacionPaginados(
            int pagina,
            int cantidad,
            string? descripcion = null,
            string? estado = null);

    }
}
