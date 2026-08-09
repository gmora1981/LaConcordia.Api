using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Reporteria;

namespace Identity.Api.Services
{
    public class BalanceServices : IBalance
    {
        private BalanceRepository _balance = new BalanceRepository();

        public BalanceResultadoDTO GetBalance(DateTime fechaDesde, DateTime fechaHasta)
        {
            return _balance.GetBalance(fechaDesde, fechaHasta);
        }

        public byte[] ExportarBalancePdf(DateTime fechaDesde, DateTime fechaHasta, string usuario)
        {
            var resultado = _balance.GetBalance(fechaDesde, fechaHasta);
            return BalancePdfGenerator.GenerarPdf(resultado, usuario);
        }
    }
}
