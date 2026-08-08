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
    }
}
