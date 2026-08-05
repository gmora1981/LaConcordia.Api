using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class GenerarPlanChoqueServices : IGenerarPlanChoque
    {
        private GenerarPlanChoqueRepository _plan = new GenerarPlanChoqueRepository();

        public bool YaFueGenerado(string unidad)
        {
            return _plan.YaFueGenerado(unidad);
        }

        public GenerarPlanResultadoDTO GenerarPlanChoque(GenerarPlanChoqueRequestDTO request)
        {
            return _plan.GenerarPlanChoque(request);
        }
    }
}
