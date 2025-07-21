using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class NiveleducacionServices : INiveleducacion
    {
        private NiveleeducacionRepository _niveleducacionRepository = new NiveleeducacionRepository();

        public IEnumerable<Niveleducacion> GetNiveleducacionInfoAll()
        {
            return _niveleducacionRepository.GetNiveleducacionInfoAll();
        }

        public NiveleducacionDTO GetNiveleducacionById(int idNiveleducacion)
        {
            return _niveleducacionRepository.GetNiveleducacionById(idNiveleducacion);
        }

        public void InsertNiveleducacion(Niveleducacion nueva)
        {
            _niveleducacionRepository.InsertNiveleducacion(nueva);
        }

        public void UpdateNiveleducacion(Niveleducacion actualizada)
        {
            _niveleducacionRepository.UpdateNiveleducacion(actualizada);
        }

        public void DeleteNiveleducacionById(int idNiveleducacion)
        {
            _niveleducacionRepository.DeleteNiveleducacionById(idNiveleducacion);
        }


    }
}
