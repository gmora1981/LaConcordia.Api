using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IFichaobservacione
    {
        IEnumerable<Fichaobservacione> GetFichaObservacioneInfoAll();
        List<FichaobservacioneDTO> GetFichaObservacioneByCedula(string cedula); // ahora devuelve lista
        void InsertFichaObservacione(FichaobservacioneDTO New);
        void UpdateFichaObservacione(FichaobservacioneDTO UpdItem);
        void DeleteFichaObservacioneByCedula(int cedula);

        // Paginado
        Task<PagedResult<Fichaobservacione>> GetFichaObservacionePaginados(
            int pagina,
            int pageSize,
            string? unidad = null,
            string? cedula = null);

        //paginado por cedula
        Task<PagedResult<Fichaobservacione>> GetFichaObservacionePaginadosByCedula(
        int pagina,
        int pageSize,
        string Fkcedula);

    }

}
