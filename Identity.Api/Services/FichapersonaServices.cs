using Identity.Api.DTO;
using Identity.Api.Paginado;

namespace Identity.Api.Services
{
    public class FichapersonaServices : IFichapersona
    {
        private readonly FichapersonaRepository _fichapersonaRepository;
        public FichapersonaServices(FichapersonaRepository fichapersonaRepository)
        {
            _fichapersonaRepository = fichapersonaRepository;
        }
        public IEnumerable<FichapersonalDTO> GetFichaPersonalInfoAll()
        {
            return _fichapersonaRepository.GetFichaPersonalInfoAll();
        }
        public FichapersonalDTO GetFichaPersonalById(int idParentesco)
        {
            return _fichapersonaRepository.GetFichaPersonalById(idParentesco);
        }
        public void InsertFichaPersonal(FichapersonalDTO New)
        {
            _fichapersonaRepository.InsertFichaPersonal(New);
        }
        public void UpdateFichaPersonal(FichapersonalDTO UpdItem)
        {
            _fichapersonaRepository.UpdateFichaPersonal(UpdItem);
        }
        public void DeleteFichaPersonalById(int idParentesco)
        {
            _fichapersonaRepository.DeleteFichaPersonalById(idParentesco);
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
