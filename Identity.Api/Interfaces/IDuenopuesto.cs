using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IDuenopuesto
    {
        IEnumerable<DuenopuestoDTO> GetDuenopuestoInfoAll();
        DuenopuestoDTO GetDuenopuestoById(string cedula);
        void InsertDuenopuesto(DuenopuestoDTO New);
        void UpdateDuenopuesto(DuenopuestoDTO UpdItem);
        void DeletePDuenopuestoById(string cedula);
    }
}
