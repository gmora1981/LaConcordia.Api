using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IBalance
    {
        // Agrupa los movimientos de cuota y UBM por forma de pago dentro del rango
        // [fechaDesde, fechaHasta] (equivalente a la consulta SE_BALANCE del sistema
        // de escritorio; aqui se calcula, no vive en una tabla de la base de datos).
        BalanceResultadoDTO GetBalance(DateTime fechaDesde, DateTime fechaHasta);
    }
}
