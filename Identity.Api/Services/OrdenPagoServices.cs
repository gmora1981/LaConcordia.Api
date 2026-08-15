using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class OrdenPagoServices : IOrdenPago
    {
        private OrdenPagoRepository _ordenPago = new OrdenPagoRepository();

        public List<PedidoVoucherDTO> GetPedidosPendientesVoucher(string? ruc = null)
        {
            return _ordenPago.GetPedidosPendientesVoucher(ruc);
        }

        public string? GetDireccionTexto(string celular, decimal lat, decimal lng)
        {
            return _ordenPago.GetDireccionTexto(celular, lat, lng);
        }

        public decimal GetSaldoCajaActual()
        {
            return _ordenPago.GetSaldoCajaActual();
        }

        public OrdenPagoResultadoDTO GenerarOrdenPago(GenerarOrdenPagoRequestDTO request, string usuario)
        {
            return _ordenPago.GenerarOrdenPago(request, usuario);
        }

        public List<OrdenPagoResumenDTO> GetOrdenPagoPorEmpresa(string ruc, DateTime? hasta = null)
        {
            return _ordenPago.GetOrdenPagoPorEmpresa(ruc, hasta);
        }

        public void ActualizarDatosPedido(ActualizarDatosPedidoRequestDTO request, string usuario)
        {
            _ordenPago.ActualizarDatosPedido(request, usuario);
        }

        public byte[] ExportarFacturacionPdf(string ruc, string razonSocial, DateTime? hasta, string usuario)
        {
            return _ordenPago.ExportarFacturacionPdf(ruc, razonSocial, hasta, usuario);
        }
    }
}
