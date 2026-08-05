using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class GenerarCuotaServices : IGenerarCuota
    {
        private GenerarCuotaRepository _generarCuota = new GenerarCuotaRepository();

        public List<PendienteCuotaDTO> GetPendientesPorPeriodo(string periodo, string semana)
        {
            return _generarCuota.GetPendientesPorPeriodo(periodo, semana);
        }

        public GenerarCuotaResultadoDTO GenerarCuotaSemanal(string periodo, string semana)
        {
            return _generarCuota.GenerarCuotaSemanal(periodo, semana);
        }

        //paginado
        public async Task<PagedResult<Generarcuotum>> GetGenerarCuotaPaginados(
            int pagina,
            int pageSize,
            string? periodo = null,
            string? semana = null,
            string? cidentidad = null)
        {
            return await _generarCuota.GetGenerarCuotaPaginados(pagina, pageSize, periodo, semana, cidentidad);
        }
    }
}
