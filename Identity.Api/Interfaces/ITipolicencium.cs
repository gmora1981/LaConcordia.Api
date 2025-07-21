using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;


namespace Identity.Api.Interfaces
{
    public interface ITipolicencium
    {
        IEnumerable<Tipolicencium> GetTipolicenciaInfoAll();
        TipolicenciumDTO? GetTipolicenciaById(int idTipoLicencia);
        void InsertTipolicencia(Tipolicencium nueva);
        void UpdateTipolicencia(Tipolicencium actualizada);
        void DeleteTipolicenciaById(int idTipoLicencia);

    }
}
