using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface ISolicitudCarrera
    {
        void CrearSolicitud(CrearSolicitudCarreraRequestDTO request, string ruc);
        List<SolicitudCarreraDTO> GetMisSolicitudes(string ruc);
        List<SolicitudCarreraDTO> GetSolicitudesPendientes();
        void MarcarConvertida(int idsolicitud, string usuario);
    }
}
