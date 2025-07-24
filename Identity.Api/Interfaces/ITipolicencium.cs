using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;


namespace Identity.Api.Interfaces
{
    public interface ITipolicencium
    {
        IEnumerable<Tipolicencium> GetTipolicenciaInfoAll();
        TipolicenciumDTO? GetTipolicenciaById(int idTipoLicencia);
        void InsertTipolicencia(TipolicenciumDTO nuevaDto);
        void UpdateTipolicencia(Tipolicencium actualizada);
        void DeleteTipolicenciaById(int idTipoLicencia);

        //paginado
        Task<PagedResult<Tipolicencium>> GetTipoLicenciumPaginados(
        int pagina,
        int pageSize,
        int? Idtipo = null,
        string? Tipolicencia = null,
        string? Profesional = null,
        string? Estado = null);
    }
}
