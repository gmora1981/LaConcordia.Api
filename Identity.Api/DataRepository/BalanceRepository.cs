using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class BalanceRepository
    {
        public BalanceResultadoDTO GetBalance(DateTime fechaDesde, DateTime fechaHasta)
        {
            using var context = new DbAa5796GmoraContext();

            // El "hasta" incluye el dia completo, igual que hacia el sistema de escritorio
            // (DateAdd un dia y comparar con '<').
            var hastaExclusivo = fechaHasta.Date.AddDays(1);

            var movCuota = context.Movimientocuota
                .Where(m => m.Fechapago >= fechaDesde.Date && m.Fechapago < hastaExclusivo)
                .Select(m => new { m.Formadepago, m.Valorpagado });

            var movUbm = context.Movimientoubms
                .Where(m => m.Fechapago >= fechaDesde.Date && m.Fechapago < hastaExclusivo)
                .Select(m => new { m.Formadepago, m.Valorpagado });

            var todos = movCuota.Concat(movUbm).ToList();

            var items = todos
                .Where(m => !string.IsNullOrEmpty(m.Formadepago))
                .GroupBy(m => m.Formadepago!.ToUpper())
                .Select(g => new BalanceItemDTO
                {
                    FormaDePago = g.Key,
                    Valor = g.Sum(x => x.Valorpagado ?? 0),
                    Registros = g.Count()
                })
                .OrderBy(x => x.FormaDePago)
                .ToList();

            return new BalanceResultadoDTO
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                Items = items
            };
        }
    }
}
