using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
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
        public void InsertTipolicencia(Tipolicencium nueva)
        {
            _tipolicencium.InsertTipolicencia(nueva);
        }
        public void UpdateTipolicencia(Tipolicencium actualizada)
        {
            _tipolicencium.UpdateTipolicencia(actualizada);
        }
        public void DeleteTipolicenciaById(int idTipoLicencia)
        {
            _tipolicencium.DeleteTipolicenciaById(idTipoLicencia);
        }

    }
}
