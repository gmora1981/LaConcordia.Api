using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class TipolicenciumRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public TipolicenciumRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Tipolicencium> GetTipolicenciaInfoAll()
        {
            return _context.Tipolicencia.Where(x => x.Estado == "a").ToList();
        }

        public TipolicenciumDTO? GetTipolicenciaById(int idTipoLicencia)
        {
            //return _context.Parentescos.FirstOrDefault(x => x.Idparentesco == id && x.Estado == "a");
            using var context = new DbAa5796GmoraContext();

            var s = context.Tipolicencia
                //.Include(f => f.IdProveedorNavigation)
                //.Include(f => f.IdBodegaNavigation)
                .FirstOrDefault(f => f.Idtipo == idTipoLicencia);

            if (s == null) return null;

            return new TipolicenciumDTO
            {
                Idtipo = s.Idtipo,
                Tipolicencia = s.Tipolicencia,
                Profesional = s.Profesional,
                Estado = s.Estado
            };
        }

        public void InsertTipolicencia(TipolicenciumDTO nuevaDto)
        {
            // Convertimos el DTO al modelo real
            var entidad = new Tipolicencium
            {
                Tipolicencia = nuevaDto.Tipolicencia?.ToUpper(), // <-- mayúsculas forzadas
                Profesional = nuevaDto.Profesional,
                Estado = nuevaDto.Estado
            };

            _context.Tipolicencia.Add(entidad);
            _context.SaveChanges();
        }


        public void UpdateTipolicencia(Tipolicencium actualizada)
        {
            _context.Tipolicencia.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteTipolicenciaById(int idTipoLicencia)
        {
            var item = _context.Tipolicencia.FirstOrDefault(x => x.Idtipo == idTipoLicencia);
            if (item != null)
            {
                _context.Tipolicencia.Remove(item);
                _context.SaveChanges();
            }

        }

        //paginado
        public async Task<PagedResult<Tipolicencium>> GetTipoLicenciumPaginados(
            int pagina,
            int pageSize,
            int? Idtipo = null,
            string? Tipolicencia = null,
            string? Profesional = null,
            string? Estado = null)
        {
            var query = _context.Tipolicencia.AsQueryable();

            if (Idtipo.HasValue)
                query = query.Where(x => x.Idtipo == Idtipo.Value);

            if (!string.IsNullOrEmpty(Tipolicencia))
                query = query.Where(x => x.Tipolicencia != null && x.Tipolicencia.Contains(Tipolicencia));

            if (!string.IsNullOrEmpty(Profesional))
                query = query.Where(x => x.Profesional != null && x.Profesional.Contains(Profesional));

            if (!string.IsNullOrEmpty(Estado))
                query = query.Where(x => x.Estado == Estado);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Idtipo)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Tipolicencium>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}   
