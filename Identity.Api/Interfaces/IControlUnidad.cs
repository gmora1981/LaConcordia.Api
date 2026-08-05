using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IControlUnidad
    {
        // estadoServicio: "a" (en servicio) o "p" (fuera de servicio)
        List<UnidadServicioDTO> GetFichaPersonalPorServicio(string estadoServicio);

        void MoverUnidad(MoverUnidadRequestDTO request, string monitora);
    }
}
