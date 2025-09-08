using Identity.Api.DTO;
using Identity.Api.Paginado;

namespace Identity.Api.Interfaces
{
    public interface IFichapersona
    {
        IEnumerable<FichapersonalDTO> GetFichaPersonalInfoAll();
        FichapersonalDTO GetFichaPersonalById(string cedula);
        FichapersonalDTO GetFichaPersonalByCorreo(string correo);
        void InsertFichaPersonal(FichapersonalDTO New);
        void UpdateFichaPersonal(FichapersonalDTO UpdItem);
        void DeleteFichaPersonalById(string cedula);

        //paginado
        Task<PagedResult<FichapersonalDTO>> GetFichaPersonalPaginados(
        int pagina,
        int pageSize,
        string? filtro = null,
        string? estado = null);
    }
}
