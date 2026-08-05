using Identity.Api.DTO;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public interface IGenerarCuota
    {
        // Socios activos que aun NO tienen cuota generada para ese Periodo+Semana
        // (equivalente a SE_FICHAPERSONALXPERIODO del sistema de escritorio).
        List<PendienteCuotaDTO> GetPendientesPorPeriodo(string periodo, string semana);

        // Genera la cuota semanal (Valor = FichaPersonal.Cuotaf) para cada socio
        // pendiente de ese Periodo+Semana. La clave primaria compuesta de la tabla
        // (Periodo+Semana+Cidentidad) impide duplicar la generacion.
        GenerarCuotaResultadoDTO GenerarCuotaSemanal(string periodo, string semana);

        //paginado
        Task<PagedResult<Generarcuotum>> GetGenerarCuotaPaginados(
            int pagina,
            int pageSize,
            string? periodo = null,
            string? semana = null,
            string? cidentidad = null);
    }
}
