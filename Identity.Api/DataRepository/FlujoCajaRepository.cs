using Identity.Api.DTO;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class FlujoCajaRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public FlujoCajaRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Flujocaja> GetFlujoCajaInfoAll()
        {
            return _context.Flujocajas.OrderByDescending(x => x.Idflujocaja).ToList();
        }

        public void InsertFlujoCaja(Flujocaja nuevo)
        {
            // El saldo corriente se calcula aqui, no se confia en lo que envie el cliente:
            // saldo nuevo = saldo del ultimo registro + ingreso - egreso.
            var ultimo = _context.Flujocajas.OrderByDescending(x => x.Idflujocaja).FirstOrDefault();
            var saldoAnterior = ultimo?.Saldo ?? 0m;

            nuevo.Saldo = saldoAnterior + (nuevo.Ingreso ?? 0m) - (nuevo.Egreso ?? 0m);
            if (nuevo.Fecha == null)
                nuevo.Fecha = DateTime.Now;

            _context.Flujocajas.Add(nuevo);
            _context.SaveChanges();
        }

        public FlujoCajaDTO? GetUltimoRegistro()
        {
            using var context = new DbAa5796GmoraContext();

            var ultimo = context.Flujocajas
                .OrderByDescending(x => x.Idflujocaja)
                .FirstOrDefault();

            if (ultimo == null) return null;

            return new FlujoCajaDTO
            {
                Idflujocaja = ultimo.Idflujocaja,
                Fecha = ultimo.Fecha,
                Concepto = ultimo.Concepto,
                Ingreso = ultimo.Ingreso,
                Egreso = ultimo.Egreso,
                Saldo = ultimo.Saldo,
                Observacion = ultimo.Observacion,
                Usuario = ultimo.Usuario
            };
        }

        //paginado
        public async Task<PagedResult<Flujocaja>> GetFlujoCajaPaginados(
            int pagina,
            int pageSize,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string? concepto = null)
        {
            var query = _context.Flujocajas.AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(x => x.Fecha >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(x => x.Fecha <= fechaHasta.Value);

            if (!string.IsNullOrEmpty(concepto))
                query = query.Where(x => x.Concepto != null && x.Concepto.Contains(concepto));

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Idflujocaja)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Flujocaja>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}
