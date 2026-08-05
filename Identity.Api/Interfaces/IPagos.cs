using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IPagos
    {
        DeudaSocioDTO? GetDeudaSocio(string cedula);

        bool ExisteComprobante(string banco, string numComprobante);

        void PagarCuota(PagoCuotaRequestDTO request, string usuario);

        void PagarUbm(PagoUbmRequestDTO request, string usuario);
    }
}
