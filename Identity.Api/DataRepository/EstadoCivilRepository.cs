using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class EstadoCivilRepository
    {
        private readonly DbAa5796GmoraContext _context;
        public EstadoCivilRepository()
        {
            _context = new DbAa5796GmoraContext();
        }
        public IEnumerable<Estadocivil> GetEstadocivilInfoAll()
        {
            return _context.Estadocivils.Where(x => x.Estado == "a").ToList();
        }
        public EstadocivilDTO GetEstadocivilById(int idEstadoCivil)
        {
            return _context.Estadocivils
                .Where(x => x.Idestadocivil == idEstadoCivil && x.Estado == "a")
                .Select(s => new EstadocivilDTO
                {
                    Idestadocivil = s.Idestadocivil,
                    Descripcion = s.Descripcion,
                    Estado = s.Estado
                })
                .FirstOrDefault();
        }
        public void InsertEstadocivil(Estadocivil nueva)
        {
            _context.Estadocivils.Add(nueva);
            _context.SaveChanges();
        }
        public void UpdateEstadocivil(Estadocivil actualizada)
        {
            _context.Estadocivils.Update(actualizada);
            _context.SaveChanges();
        }
        public void DeleteEstadocivilById(int idEstadoCivil)
        {
            var item = _context.Estadocivils.FirstOrDefault(x => x.Idestadocivil == idEstadoCivil);
            if (item != null)
            {
                _context.Estadocivils.Remove(item);
                _context.SaveChanges();
            }
        }

        //paginado
        public async Task<PagedResult<Estadocivil>> GetEstadoCivilPaginados(
            int pagina,
            int pageSize,
            string? descripcion = null,
            string? estado = null)
        {
            var query = _context.Estadocivils.AsQueryable();

            if (!string.IsNullOrEmpty(descripcion))
                query = query.Where(x => x.Descripcion.Contains(descripcion));

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(x => x.Estado == estado);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Descripcion)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Estadocivil>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}
