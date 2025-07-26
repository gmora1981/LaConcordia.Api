using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
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

        //paginado
        public async Task<PagedResult<Duenopuesto>> GetDuenopuestosPaginados(
            int pagina,
            int pageSize,
            string? cedula = null,
            string? nombre = null,
            string? apellidos = null,
            string? estado = null)
        {
            return await _duenopuesto.GetDuenopuestosPaginados(pagina, pageSize, cedula, nombre, apellidos, estado);
        }
    }

}
