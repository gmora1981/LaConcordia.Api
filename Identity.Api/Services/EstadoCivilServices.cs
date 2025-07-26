using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class EstadoCivilServices : IEstadoCivil
    {
        private readonly EstadoCivilRepository _estadoCivilRepository = new EstadoCivilRepository();

        public IEnumerable<Estadocivil> GetEstadocivilInfoAll()
        {
            return _estadoCivilRepository.GetEstadocivilInfoAll();
        }
        public EstadocivilDTO GetEstadocivilById(int idEstadoCivil)
        {
            return _estadoCivilRepository.GetEstadocivilById(idEstadoCivil);
        }
        public void InsertEstadocivil(Estadocivil newEstadoCivil)
        {
            _estadoCivilRepository.InsertEstadocivil(newEstadoCivil);
        }
        public void UpdateEstadocivil(Estadocivil updItem)
        {
            _estadoCivilRepository.UpdateEstadocivil(updItem);
        }
        public void DeleteEstadocivilById(int idEstadoCivil)
        {
            _estadoCivilRepository.DeleteEstadocivilById(idEstadoCivil);
        }

        //paginado
        public async Task<PagedResult<Estadocivil>> GetEstadoCivilPaginados(int pagina, int pageSize, string? descripcion = null, string? estado = null)
        {
            return await _estadoCivilRepository.GetEstadoCivilPaginados(pagina, pageSize, descripcion, estado);
        }
    }
}
