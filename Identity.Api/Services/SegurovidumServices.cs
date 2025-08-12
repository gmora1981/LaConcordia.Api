using Identity.Api.DataRepository;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class SegurovidumServices : ISegurovidum
    {
        private SegurovidumRepository segurovidumRepository = new SegurovidumRepository();

        public IEnumerable<Modelo.laconcordia.Modelo.Database.Segurovidum> GetSegurovidumInfoAll()
        {
            return segurovidumRepository.GetSegurovidumInfoAll();
        }

        public List<DTO.SegurovidumDTO> GetSegurovidumByCedula(string CiAfiliado)
        {
            return segurovidumRepository.GetSegurovidumByCedula(CiAfiliado);
        }
        public void InsertSegurovidum(DTO.SegurovidumDTO New)
        {
            segurovidumRepository.InsertSegurovidum(New);
        }

        public void UpdateSegurovidum(DTO.SegurovidumDTO UpdItem)
        {
            segurovidumRepository.UpdateSegurovidum(UpdItem);
        }
        public void DeleteSegurovidumByCedula(string CiBeneficiario)
        {
            segurovidumRepository.DeleteSegurovidumByCedula(CiBeneficiario);
        }
        // Paginado
        public async Task<Paginado.PagedResult<Modelo.laconcordia.Modelo.Database.Segurovidum>> GetSegurovidumPaginados(
            int pagina,
            int pageSize,
            string? CiBeneficiario = null,
            string? CiAfiliado = null)
        {
            return await segurovidumRepository.GetSegurovidumPaginados(pagina, pageSize, CiBeneficiario, CiAfiliado);
        }

        //paginado por cedula afiliado
        public async Task<Paginado.PagedResult<Modelo.laconcordia.Modelo.Database.Segurovidum>> GetSegurovidumPaginadosByCedulaAfiliado(
            int pagina,
            int pageSize,
            string CiAfiliado)
        {
            return await segurovidumRepository.GetSegurovidumPaginadosByCedulaAfiliado(pagina, pageSize, CiAfiliado);
        }

    }
}
