using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class NiveleeducacionRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public NiveleeducacionRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Niveleducacion> GetNiveleducacionInfoAll()
        {
            return _context.Niveleducacions.Where(x => x.Estado == "a").ToList();
        }

        public NiveleducacionDTO? GetNiveleducacionById(int idNiveleducacion)
        {
            var s = _context.Niveleducacions
                .FirstOrDefault(f => f.Ideducacion == idNiveleducacion);
            if (s == null) return null;
            return new NiveleducacionDTO
            {
                Ideducacion = s.Ideducacion,
                Descripcion = s.Descripcion,
                Estado = s.Estado
            };
        }

        public void InsertNiveleducacion(Niveleducacion nueva)
        {
            _context.Niveleducacions.Add(nueva);
            _context.SaveChanges();
        }

        public void UpdateNiveleducacion(Niveleducacion actualizada)
        {
            _context.Niveleducacions.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteNiveleducacionById(int idNiveleducacion)
        {
            var item = _context.Niveleducacions.FirstOrDefault(x => x.Ideducacion == idNiveleducacion);
            if (item != null)
            {
                _context.Niveleducacions.Remove(item);
                _context.SaveChanges();
            }
        }

        //paginado
        public async Task<PagedResult<Niveleducacion>> GetNiveleducacionPaginados(
            int pagina,
            int pageSize,
            string? descripcion = null,
            string? estado = null)
        {
            var query = _context.Niveleducacions.AsQueryable();
            if (!string.IsNullOrEmpty(descripcion))
            {
                query = query.Where(x => x.Descripcion.Contains(descripcion));
            }
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(x => x.Estado == estado);
            }
            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Descripcion) // Ordenar por descripcion
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Niveleducacion>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}
