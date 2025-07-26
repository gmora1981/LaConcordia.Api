using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IEstadoCivil
    {
        IEnumerable<Estadocivil> GetEstadocivilInfoAll();
        EstadocivilDTO GetEstadocivilById(int idestadocivil);
        void InsertEstadocivil(Estadocivil New);
        void UpdateEstadocivil(Estadocivil UpdItem);
        void DeleteEstadocivilById(int idestadocivil);

        //paginado
        Task<PagedResult<Estadocivil>> GetEstadoCivilPaginados(
        int pagina,
        int pageSize,
        string? descripcion = null,
        string? estado = null);
    }
}
