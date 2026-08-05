using Identity.Api.DTO;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class GenerarCuotaRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public GenerarCuotaRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public List<PendienteCuotaDTO> GetPendientesPorPeriodo(string periodo, string semana)
        {
            using var context = new DbAa5796GmoraContext();

            var yaGenerados = context.Generarcuota
                .Where(c => c.Periodo == periodo && c.Semana == semana)
                .Select(c => c.Cidentidad);

            return context.Fichapersonals
                .Where(f => f.Estado == "a" && !yaGenerados.Contains(f.Cedula))
                .Select(f => new PendienteCuotaDTO
                {
                    Cedula = f.Cedula,
                    Nombre = f.Nombre,
                    Apellidos = f.Apellidos,
                    Cuotaf = f.Cuotaf
                })
                .OrderBy(f => f.Apellidos)
                .ToList();
        }

        public GenerarCuotaResultadoDTO GenerarCuotaSemanal(string periodo, string semana)
        {
            var pendientes = GetPendientesPorPeriodo(periodo, semana);
            var resultado = new GenerarCuotaResultadoDTO();

            foreach (var socio in pendientes)
            {
                var nuevo = new Generarcuotum
                {
                    Periodo = periodo,
                    Semana = semana,
                    Cidentidad = socio.Cedula,
                    Valor = socio.Cuotaf ?? 0m,
                    Fecha = DateOnly.FromDateTime(DateTime.Now),
                    Abono = 0m
                };

                _context.Generarcuota.Add(nuevo);
                resultado.CedulasGeneradas.Add(socio.Cedula);
            }

            _context.SaveChanges();
            resultado.TotalGenerados = resultado.CedulasGeneradas.Count;
            return resultado;
        }

        //paginado
        public async Task<PagedResult<Generarcuotum>> GetGenerarCuotaPaginados(
            int pagina,
            int pageSize,
            string? periodo = null,
            string? semana = null,
            string? cidentidad = null)
        {
            var query = _context.Generarcuota.AsQueryable();

            if (!string.IsNullOrEmpty(periodo))
                query = query.Where(x => x.Periodo == periodo);

            if (!string.IsNullOrEmpty(semana))
                query = query.Where(x => x.Semana == semana);

            if (!string.IsNullOrEmpty(cidentidad))
                query = query.Where(x => x.Cidentidad.Contains(cidentidad));

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Fecha)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Generarcuotum>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}
