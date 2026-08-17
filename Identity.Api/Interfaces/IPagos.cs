using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IPagos
    {
        DeudaSocioDTO? GetDeudaSocio(string cedula);

        bool ExisteComprobante(string banco, string numComprobante);

        void PagarCuota(PagoCuotaRequestDTO request, string usuario);

        void PagarUbm(PagoUbmRequestDTO request, string usuario);

        // "Reporte Detalle de Pagos x Monitoria": pagos de cuota semanal por unidad y rango de fechas.
        List<DetallePagoMonitoriaDTO> GetDetallePagosPorUnidad(string unidad, DateTime desde, DateTime hasta);
        byte[] ExportarReporteDetallePagosPdf(string unidad, DateTime desde, DateTime hasta, string usuario);

        // Dashboard "Cobros de Monitoria".
        ResumenMonitoriaDTO GetResumenMonitoria(DateTime desde, DateTime hasta);
    }
}
