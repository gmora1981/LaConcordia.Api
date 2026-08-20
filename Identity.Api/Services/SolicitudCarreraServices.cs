using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class SolicitudCarreraServices : ISolicitudCarrera
    {
        private SolicitudCarreraRepository _solicitud = new SolicitudCarreraRepository();

        public void CrearSolicitud(CrearSolicitudCarreraRequestDTO request, string ruc)
        {
            _solicitud.CrearSolicitud(request, ruc);
        }

        public List<SolicitudCarreraDTO> GetMisSolicitudes(string ruc)
        {
            return _solicitud.GetMisSolicitudes(ruc);
        }

        public List<SolicitudCarreraDTO> GetSolicitudesPendientes()
        {
            return _solicitud.GetSolicitudesPendientes();
        }

        public void MarcarConvertida(int idsolicitud, string usuario)
        {
            _solicitud.MarcarConvertida(idsolicitud, usuario);
        }
    }
}
