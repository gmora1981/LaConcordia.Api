using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class ControlUnidadServices : IControlUnidad
    {
        private ControlUnidadRepository _controlUnidad = new ControlUnidadRepository();

        public List<UnidadServicioDTO> GetFichaPersonalPorServicio(string estadoServicio)
        {
            return _controlUnidad.GetFichaPersonalPorServicio(estadoServicio);
        }

        public void MoverUnidad(MoverUnidadRequestDTO request, string monitora)
        {
            _controlUnidad.MoverUnidad(request, monitora);
        }

        public List<ControlUnidadMovimientoDTO> GetMovimientos(DateTime fecha, string? turno)
        {
            return _controlUnidad.GetMovimientos(fecha, turno);
        }

        public List<string> GetMonitorasDisponibles()
        {
            return _controlUnidad.GetMonitorasDisponibles();
        }

        public List<ControlUnidadMovimientoDTO> GetMovimientosPorRango(DateTime desde, DateTime hasta, string? monitora)
        {
            return _controlUnidad.GetMovimientosPorRango(desde, hasta, monitora);
        }

        public byte[] ExportarReporteIngresoSalidaPdf(DateTime desde, DateTime hasta, string? monitora, string usuario)
        {
            return _controlUnidad.ExportarReporteIngresoSalidaPdf(desde, hasta, monitora, usuario);
        }
    }
}
