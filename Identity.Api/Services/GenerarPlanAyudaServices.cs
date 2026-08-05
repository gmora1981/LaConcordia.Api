using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class GenerarPlanAyudaServices : IGenerarPlanAyuda
    {
        private GenerarPlanAyudaRepository _plan = new GenerarPlanAyudaRepository();

        public List<BeneficiarioDTO> GetBeneficiariosPorAfiliado(string ciAfiliado)
        {
            return _plan.GetBeneficiariosPorAfiliado(ciAfiliado);
        }

        public bool YaFueGenerado(string beneficiario)
        {
            return _plan.YaFueGenerado(beneficiario);
        }

        public GenerarPlanResultadoDTO GenerarPlanAyuda(GenerarPlanAyudaRequestDTO request)
        {
            return _plan.GenerarPlanAyuda(request);
        }
    }
}
