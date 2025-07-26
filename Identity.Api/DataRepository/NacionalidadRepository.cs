using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class NacionalidadRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public NacionalidadRepository()
        {
            _context = new DbAa5796GmoraContext();
        }
        public IEnumerable<Nacionalidad> NacionalidadInfoAll()
        {
            return _context.Nacionalidads.Where(x => x.Estado == "a").ToList();
        }

        public NacionalidadDTO? GetNacionalidadById(int idNacionalidad)
        {
            //return _context.Parentescos.FirstOrDefault(x => x.Idparentesco == id && x.Estado == "a");
            using var context = new DbAa5796GmoraContext();

            var s = context.Nacionalidads
                //.Include(f => f.IdProveedorNavigation)
                //.Include(f => f.IdBodegaNavigation)
                .FirstOrDefault(f => f.Idnacionalidad == idNacionalidad);

            if (s == null) return null;

            return new NacionalidadDTO
            {
                Idnacionalidad = s.Idnacionalidad,
                Nacionalidad1 = s.Nacionalidad1,
                Estado = s.Estado
            };
        }

        public void InsertNacionalidad(Nacionalidad nueva)
        {
            _context.Nacionalidads.Add(nueva);
            _context.SaveChanges();
        }

        public void UpdateNacionalidad(Nacionalidad actualizada)
        {
            _context.Nacionalidads.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteNacionalidadById(int idNacionalidad)
        {
            var item = _context.Nacionalidads.FirstOrDefault(x => x.Idnacionalidad == idNacionalidad);
            if (item != null)
            {
                _context.Nacionalidads.Remove(item);
                _context.SaveChanges();
            }
        }


        //paginado
        public async Task<PagedResult<Nacionalidad>> GetNacionalPaginados(
            int pagina,
            int pageSize,
            string? nacionalidad = null,

            string? estado = null)
        {
            var query = _context.Nacionalidads.AsQueryable();

            if (!string.IsNullOrEmpty(nacionalidad))
                query = query.Where(x => x.Nacionalidad1.Contains(nacionalidad));

            

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(x => x.Estado == estado);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Nacionalidad1)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Nacionalidad>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}
