using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class UnidadDataRepository
    {

        private readonly DbAa5796GmoraContext _context;

        public UnidadDataRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Unidad> UnidadAll()
        {
            return _context.Unidads.Where(x => x.Estado == "a").ToList();
        }

        public IEnumerable<Unidad> GetUnidadById(string idUnidad)
        {
            return _context.Unidads.Where(x => x.Unidad1 == idUnidad && x.Estado == "a").ToList();
        }

        public void InsertUnidad(UnidadDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var nuevaUnidad = new Unidad
            {
                Unidad1 = dto.Unidad1?.Trim().ToUpper(),
                Placa = dto.Placa?.Trim().ToUpper(),
                Idpropietario = dto.Idpropietario?.Trim().ToUpper(),
                Propietario = dto.Propietario?.Trim().ToUpper(),
                Marca = dto.Marca?.Trim().ToUpper(),
                Modelo = dto.Modelo?.Trim().ToUpper(),
                Anio = dto.Anio,
                Estado = dto.Estado,
                Color = dto.Color?.Trim().ToUpper(),
            };

            _context.Unidads.Add(nuevaUnidad);
            _context.SaveChanges();
        }

        public void UpdateUnidad(Unidad actualizada)
        {
            _context.Unidads.Update(actualizada);
            _context.SaveChanges();
        }

        public void DeleteUnidadById(string idUnidad)
        {
            var item = _context.Unidads.FirstOrDefault(x => x.Unidad1 == idUnidad);
            if (item != null)
            {
                _context.Unidads.Remove(item);
                _context.SaveChanges();
            }
        }

        //paginado
        public async Task<PagedResult<UnidadDTO>> GetUnidadPaginados(
            int pagina,
            int pageSize,
            string? Placa = null,
            string? Idpropietario = null,
            string? Unidad1 = null,
            string? Propietario = null,
            string? Estado = null)
        {
            var query = _context.Unidads.AsQueryable();
            if (!string.IsNullOrEmpty(Placa))
                query = query.Where(x => x.Placa.Contains(Placa));
            if (!string.IsNullOrEmpty(Idpropietario))
                query = query.Where(x => x.Idpropietario.Contains(Idpropietario));
            if (!string.IsNullOrEmpty(Unidad1))
                query = query.Where(x => x.Unidad1.Contains(Unidad1));
            if (!string.IsNullOrEmpty(Propietario))
                query = query.Where(x => x.Propietario.Contains(Propietario));
            if (!string.IsNullOrEmpty(Estado))
                query = query.Where(x => x.Estado == Estado);
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new UnidadDTO
                {
                    Unidad1 = x.Unidad1,
                    Placa = x.Placa,
                    Idpropietario = x.Idpropietario,
                    Propietario = x.Propietario,
                    Estado = x.Estado
                })
                .ToListAsync();
            return new PagedResult<UnidadDTO>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }
    }
}
