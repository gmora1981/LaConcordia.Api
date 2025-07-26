using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class ParentescoRepository 
    {
        private readonly DbAa5796GmoraContext _context;

        public ParentescoRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Parentesco> ParentescoInfoAll()
        {
            return _context.Parentescos.Where(x => x.Estado == "a").ToList();
        }

        public ParentescoDTO? GetParentescoById(int idParentesco)
        {
            //return _context.Parentescos.FirstOrDefault(x => x.Idparentesco == id && x.Estado == "a");
            using var context = new DbAa5796GmoraContext();

            var s = context.Parentescos
                //.Include(f => f.IdProveedorNavigation)
                //.Include(f => f.IdBodegaNavigation)
                .FirstOrDefault(f => f.Idparentesco == idParentesco);

            if (s == null) return null;

            return new ParentescoDTO
            {
                Idparentesco = s.Idparentesco,
                Parentesco1 = s.Parentesco1,
                Estado = s.Estado
            };
        }

        public void InsertParentesco(Parentesco nueva)
        {
            _context.Parentescos.Add(nueva);
            _context.SaveChanges();
        }

        public void UpdateParentesco(Parentesco actualizada)
        {
            _context.Parentescos.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteParentescoById(int idParentesco)
        {
            var item = _context.Parentescos.FirstOrDefault(x => x.Idparentesco == idParentesco);
            if (item != null)
            {
                _context.Parentescos.Remove(item);
                _context.SaveChanges();
            }
        }

        public async Task<PagedResult<Parentesco>> GetParentescoPaginados(
            int pagina,
            int pageSize,
            string? parentesco1 = null,
            string? estado = null)
        {
            var query = _context.Parentescos.AsQueryable();
            if (!string.IsNullOrEmpty(parentesco1))
            {
                query = query.Where(x => x.Parentesco1.Contains(parentesco1));
            }
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(x => x.Estado == estado);
            }
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Parentesco>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}
