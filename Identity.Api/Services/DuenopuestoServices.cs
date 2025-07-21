using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class DuenopuestoServices : IDuenopuesto
    {
        private DuenopuestoRepository _duenopuesto = new DuenopuestoRepository();


        public IEnumerable<DuenopuestoDTO> GetDuenopuestoInfoAll()
        {
            return _duenopuesto.GetDuenopuestoInfoAll();
        }

        public DuenopuestoDTO GetDuenopuestoById(string cedula)
        {
            return _duenopuesto.GetDuenopuestoById(cedula);
        }

        public void InsertDuenopuesto(DuenopuestoDTO New)
        {
            _duenopuesto.InsertDuenopuesto(New);
        }

        public void UpdateDuenopuesto(DuenopuestoDTO UpdItem)
        {
            _duenopuesto.UpdateDuenopuesto(UpdItem);
        }

        public void DeletePDuenopuestoById(string cedula)
        {
            _duenopuesto.DeletePDuenopuestoById(cedula);
        }
    }
}
