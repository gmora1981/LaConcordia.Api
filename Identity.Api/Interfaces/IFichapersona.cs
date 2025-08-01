using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IFichapersona
    {
        IEnumerable<FichapersonalDTO> GetFichaPersonalInfoAll();
        FichapersonalDTO GetFichaPersonalById(string idParentesco);
        void InsertFichaPersonal(FichapersonalDTO New);
        void UpdateFichaPersonal(FichapersonalDTO UpdItem);
        void DeleteFichaPersonalById(string idParentesco);

        //paginado
        Task<PagedResult<FichapersonalDTO>> GetFichaPersonalPaginados(
        int pagina,
        int pageSize,
        string? filtro = null,
        string? estado = null);
    }
}
