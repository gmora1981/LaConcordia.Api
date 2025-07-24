using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class TipolicenciumServices : ITipolicencium
    {
        private TipolicenciumRepository _tipolicencium = new TipolicenciumRepository();

        public IEnumerable<Tipolicencium> GetTipolicenciaInfoAll()
        {
            return _tipolicencium.GetTipolicenciaInfoAll();
        }
        
        public TipolicenciumDTO? GetTipolicenciaById(int idTipoLicencia)
        {
            return _tipolicencium.GetTipolicenciaById(idTipoLicencia);
        }
        public void InsertTipolicencia(TipolicenciumDTO nuevaDto)
        {
            _tipolicencium.InsertTipolicencia(nuevaDto);
        }
        public void UpdateTipolicencia(Tipolicencium actualizada)
        {
            _tipolicencium.UpdateTipolicencia(actualizada);
        }
        public void DeleteTipolicenciaById(int idTipoLicencia)
        {
            _tipolicencium.DeleteTipolicenciaById(idTipoLicencia);
        }

        //paginado
        public async Task<PagedResult<Tipolicencium>> GetTipoLicenciumPaginados(int pagina,
        int pageSize,
        int? Idtipo = null,
        string? Tipolicencia = null,
        string? Profesional = null,
        string? Estado = null)
        {
            return await _tipolicencium.GetTipoLicenciumPaginados(pagina, pageSize, Idtipo, Tipolicencia, Profesional, Estado);
        }

    }
}
