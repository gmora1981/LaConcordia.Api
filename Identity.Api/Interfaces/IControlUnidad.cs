using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IControlUnidad
    {
        // estadoServicio: "a" (en servicio) o "p" (fuera de servicio)
        List<UnidadServicioDTO> GetFichaPersonalPorServicio(string estadoServicio);

        void MoverUnidad(MoverUnidadRequestDTO request, string monitora);

        List<ControlUnidadMovimientoDTO> GetMovimientos(DateTime fecha, string? turno);

        // "Reporte de Ingreso y Salida" por operadora/monitora y/o unidad, y rango de fechas.
        List<string> GetMonitorasDisponibles();
        List<string> GetUnidadesConMovimientos();
        List<ControlUnidadMovimientoDTO> GetMovimientosPorRango(DateTime desde, DateTime hasta, string? monitora, string? unidad = null);
        byte[] ExportarReporteIngresoSalidaPdf(DateTime desde, DateTime hasta, string? monitora, string? unidad, string usuario);
    }
}
