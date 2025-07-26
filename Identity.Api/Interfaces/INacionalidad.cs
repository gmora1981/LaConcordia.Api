using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface INacionalidad
    {
        IEnumerable<Nacionalidad> GetNacionalidadInfoAll();
        NacionalidadDTO? GetNacionalidadById(int idNacionalidad);
        void InsertNacionalidad(Nacionalidad nueva);
        void UpdateNacionalidad(Nacionalidad actualizada);
        void DeleteNacionalidadById(int idNacionalidad);

        Task<PagedResult<Nacionalidad>> GetNacionalPaginados(
        int pagina,
        int pageSize,
        string? nacionalidad = null,
        string? estado = null);
    }
}
