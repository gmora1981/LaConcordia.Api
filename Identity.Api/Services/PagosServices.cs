using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class PagosServices : IPagos
    {
        private PagosRepository _pagos = new PagosRepository();

        public DeudaSocioDTO? GetDeudaSocio(string cedula)
        {
            return _pagos.GetDeudaSocio(cedula);
        }

        public bool ExisteComprobante(string banco, string numComprobante)
        {
            return _pagos.ExisteComprobante(banco, numComprobante);
        }

        public void PagarCuota(PagoCuotaRequestDTO request, string usuario)
        {
            _pagos.PagarCuota(request, usuario);
        }

        public void PagarUbm(PagoUbmRequestDTO request, string usuario)
        {
            _pagos.PagarUbm(request, usuario);
        }
    }
}
