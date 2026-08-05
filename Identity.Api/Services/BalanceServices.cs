using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;

namespace Identity.Api.Services
{
    public class BalanceServices : IBalance
    {
        private BalanceRepository _balance = new BalanceRepository();

        public BalanceResultadoDTO GetBalance(DateTime fechaDesde, DateTime fechaHasta)
        {
            return _balance.GetBalance(fechaDesde, fechaHasta);
        }
    }
}
