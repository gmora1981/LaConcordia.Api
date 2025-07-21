using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
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
    }
}
