using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class GenerarMultaServices : IGenerarMulta
    {
        private GenerarMultaRepository _generarMulta = new GenerarMultaRepository();

        public List<GenerarMultaDTO> GetMultasPorSocio(string cidentidad)
        {
            return _generarMulta.GetMultasPorSocio(cidentidad);
        }

        public GenerarMultaDTO InsertGenerarMulta(GenerarMultaDTO nueva)
        {
            return _generarMulta.InsertGenerarMulta(nueva);
        }
    }
}
