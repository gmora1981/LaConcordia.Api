using System.Security.AccessControl;
using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class FichaobservacioneService : IFichaobservacione
    {
        private FichaobservacioneRepository _fichaObservacioneRepository = new FichaobservacioneRepository();

        public IEnumerable<Fichaobservacione> GetFichaObservacioneInfoAll()
        {
            return _fichaObservacioneRepository.GetFichaObservacioneInfoAll();
        }

        public List<FichaobservacioneDTO> GetFichaObservacioneByCedula(string cedula)
        {
            return _fichaObservacioneRepository.GetFichaObservacioneByCedula(cedula);
        }


        public void InsertFichaObservacione(FichaobservacioneDTO New)
        {
            _fichaObservacioneRepository.InsertFichaObservacione(New);
        }

        public void UpdateFichaObservacione(FichaobservacioneDTO UpdItem)
        {
            _fichaObservacioneRepository.UpdateFichaObservacione(UpdItem);
        }


        public void DeleteFichaObservacioneByCedula(int cedula)
        {
            _fichaObservacioneRepository.DeleteFichaObservacioneByCedula(cedula);
        }

        // Paginado
        public async Task<PagedResult<Fichaobservacione>> GetFichaObservacionePaginados(
            int pagina,
            int pageSize,
            string? unidad = null,
            string? cedula = null)
        {
            return await _fichaObservacioneRepository.GetFichaObservacionePaginados(pagina, pageSize, unidad, cedula);

        }

        //paginado por cedula
        public async Task<PagedResult<Fichaobservacione>> GetFichaObservacionePaginadosByCedula(
            int pagina,
            int pageSize,
            string Fkcedula)
        {
            return await _fichaObservacioneRepository.GetFichaObservacionePaginadosByCedula(pagina, pageSize, Fkcedula);
        }
    }
}
