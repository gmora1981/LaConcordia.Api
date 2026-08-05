using Identity.Api.DTO;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class PasajeroRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public PasajeroRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Pasajero> GetPasajeroInfoAll()
        {
            return _context.Pasajeros.ToList();
        }

        public PasajeroDTO GetPasajeroByCelular(string celular)
        {
            using var context = new DbAa5796GmoraContext();

            var s = context.Pasajeros.FirstOrDefault(f => f.Celular == celular);

            if (s == null) return null;

            return new PasajeroDTO
            {
                Celular = s.Celular,
                Nombres = s.Nombres,
                Observacion = s.Observacion,
                Correo = s.Correo,
                Sexo = s.Sexo,
                Whatsapp = s.Whatsapp,
                Publicidad = s.Publicidad
            };
        }

        public void InsertPasajero(Pasajero nuevo)
        {
            _context.Pasajeros.Add(nuevo);
            _context.SaveChanges();
        }

        public void UpdatePasajero(Pasajero actualizado)
        {
            _context.Pasajeros.Update(actualizado);
            _context.SaveChanges();
        }

        public void DeletePasajeroByCelular(string celular)
        {
            var item = _context.Pasajeros.FirstOrDefault(x => x.Celular == celular);
            if (item != null)
            {
                _context.Pasajeros.Remove(item);
                _context.SaveChanges();
            }
        }

        //paginado
        public async Task<PagedResult<Pasajero>> GetPasajeroPaginados(
            int pagina,
            int pageSize,
            string? nombres = null,
            string? celular = null)
        {
            var query = _context.Pasajeros.AsQueryable();

            if (!string.IsNullOrEmpty(nombres))
                query = query.Where(x => x.Nombres != null && x.Nombres.Contains(nombres));

            if (!string.IsNullOrEmpty(celular))
                query = query.Where(x => x.Celular.Contains(celular));

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Nombres)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Pasajero>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}
