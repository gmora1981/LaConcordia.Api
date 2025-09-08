using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;

namespace Identity.Api.Services
{
    public class FichapersonaServices : IFichapersona
    {
        private FichapersonaRepository _fichapersonaRepository = new FichapersonaRepository();
        public IEnumerable<FichapersonalDTO> GetFichaPersonalInfoAll()
        {
            return _fichapersonaRepository.GetFichaPersonalInfoAll();
        }
        public FichapersonalDTO GetFichaPersonalById(string cedula)
        {
            return _fichapersonaRepository.GetFichaPersonalById(cedula);
        }
        public FichapersonalDTO GetFichaPersonalByCorreo(string correo)
        {
            return _fichapersonaRepository.GetFichaPersonalByCorreo(correo);
        }
        public void InsertFichaPersonal(FichapersonalDTO New)
        {
            _fichapersonaRepository.InsertFichaPersonal(New);
        }
        public void UpdateFichaPersonal(FichapersonalDTO UpdItem)
        {
            _fichapersonaRepository.UpdateFichaPersonal(UpdItem);
        }
        public void DeleteFichaPersonalById(string cedula)
        {
            _fichapersonaRepository.DeleteFichaPersonalById(cedula);
        }
        //paginado
        public async Task<PagedResult<FichapersonalDTO>> GetFichaPersonalPaginados(
            int pagina,
            int pageSize,
            string? filtro = null,
            string? estado = null)
        {
            return await _fichapersonaRepository.GetFichaPersonalPaginados(pagina, pageSize, filtro, estado);
        }
    }
}
