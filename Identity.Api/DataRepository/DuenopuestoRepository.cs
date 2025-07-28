using Identity.Api.DTO;
using Identity.Api.Paginado;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class DuenopuestoRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public DuenopuestoRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<DuenopuestoDTO> GetDuenopuestoInfoAll()
        {
            return _context.Duenopuestos
        .Where(x => x.Estado == "a")
        .Select(s => new DuenopuestoDTO
        {
            Cedula = s.Cedula,
            Nombres = s.Nombres,
            Apellidos = s.Apellidos,
            Estado = s.Estado
        })
        .ToList();
        }

        public DuenopuestoDTO GetDuenopuestoById(string cedula)
        {
            //return _context.Parentescos.FirstOrDefault(x => x.Idparentesco == id && x.Estado == "a");
            using var context = new DbAa5796GmoraContext();

            var s = context.Duenopuestos
                //.Include(f => f.IdProveedorNavigation)
                //.Include(f => f.IdBodegaNavigation)
                .FirstOrDefault(f => f.Cedula == cedula && f.Estado == "a");

            if (s == null) return null;

            return new DuenopuestoDTO
            {
                Cedula = s.Cedula,
                Nombres = s.Nombres,
                Apellidos = s.Apellidos,
                Estado = s.Estado
            };
        }

        public void InsertDuenopuesto(DuenopuestoDTO dto)
        {
            var entity = new Duenopuesto
            {
                Cedula = dto.Cedula,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Estado = dto.Estado
            };

            _context.Duenopuestos.Add(entity);
            _context.SaveChanges();
        }


        public void UpdateDuenopuesto(DuenopuestoDTO dto)
        {
            var entity = _context.Duenopuestos.FirstOrDefault(x => x.Cedula == dto.Cedula);
            if (entity != null)
            {
                entity.Nombres = dto.Nombres;
                entity.Apellidos = dto.Apellidos;
                entity.Estado = dto.Estado;

                _context.Duenopuestos.Update(entity);
                _context.SaveChanges();
            }
        }


        public void DeletePDuenopuestoById(string cedula)
        {
            var item = _context.Duenopuestos.FirstOrDefault(x => x.Cedula == cedula);
            if (item != null)
            {
                _context.Duenopuestos.Remove(item);
                _context.SaveChanges();
            }
        }

        // Paginado

        public async Task<PagedResult<Duenopuesto>> GetDuenopuestosPaginados(
            int pagina,
            int pageSize,
            string? cedula = null,
            string? nombre = null,
            string? apellidos = null,
            string? estado = null)
        {
            var query = _context.Duenopuestos.AsQueryable();
            if (!string.IsNullOrEmpty(cedula))
                query = query.Where(x => x.Cedula.Contains(cedula));
            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(x => x.Nombres.Contains(nombre));
            if (!string.IsNullOrEmpty(apellidos))
                query = query.Where(x => x.Apellidos.Contains(apellidos));
            if (!string.IsNullOrEmpty(estado))
                query = query.Where(x => x.Estado == estado);
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Duenopuesto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }

        // Exportar
        public List<Duenopuesto> ObtenerDuenoPuestoFiltradas(string? filtro)
        {
            var query = _context.Duenopuestos.AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(x => x.Cedula.Contains(filtro) ||
                                         x.Nombres.Contains(filtro) ||
                                         x.Apellidos.Contains(filtro));
            }
            return query.ToList();
        }
    }
}
