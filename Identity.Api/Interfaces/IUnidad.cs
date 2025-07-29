using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IUnidad
    {
        IEnumerable<Unidad> GetUnidadInfoAll();
        Unidad? GetUnidadById(string idUnidad);
        void InsertUnidad(UnidadDTO nueva);
        void UpdateUnidad(Unidad actualizada);
        void DeleteUnidadById(string idUnidad);

        //paginado
        Task<PagedResult<UnidadDTO>> GetUnidadPaginados(
        int pagina,
        int pageSize,
        string? Placa = null,
        string? Idpropietario = null,
        string? Unidad1 = null,
        string? Propietario = null,
        string? Estado = null);

    }
}
