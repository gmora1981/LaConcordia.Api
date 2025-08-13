using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class SegurovidumRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public SegurovidumRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Segurovidum> GetSegurovidumInfoAll()
        {
            return _context.Segurovida.ToList();
        }

        public List<DTO.SegurovidumDTO> GetSegurovidumByCedula(string CiAfiliado)
        {
            return _context.Segurovida
                .Where(s => s.CiAfiliado == CiAfiliado)
                .Select(s => new DTO.SegurovidumDTO
                {
                    CiBeneficiario = s.CiBeneficiario,
                    Pkparentesco = s.Pkparentesco,
                    Nombres = s.Nombres,
                    Apellidos = s.Apellidos,
                    CiAfiliado = s.CiAfiliado,
                    Telefono = s.Telefono,
                    Tipo = s.Tipo,
                    Estado = s.Estado
                })
                .ToList();
        }

        public void InsertSegurovidum(DTO.SegurovidumDTO New)
        {
            var newSeguro = new Segurovidum
            {
                CiBeneficiario = New.CiBeneficiario,
                Pkparentesco = New.Pkparentesco,
                Nombres = New.Nombres,
                Apellidos = New.Apellidos,
                CiAfiliado = New.CiAfiliado,
                Telefono = New.Telefono,
                Tipo = New.Tipo,
                Estado = New.Estado
            };
            _context.Segurovida.Add(newSeguro);
            _context.SaveChanges();
        }

        public void UpdateSegurovidum(DTO.SegurovidumDTO UpdItem)
        {
            var existingSeguro = _context.Segurovida
                .FirstOrDefault(s => s.CiBeneficiario == UpdItem.CiBeneficiario && s.CiAfiliado == UpdItem.CiAfiliado);
            if (existingSeguro != null)
            {
                existingSeguro.Pkparentesco = UpdItem.Pkparentesco;
                existingSeguro.CiBeneficiario = UpdItem.CiBeneficiario;
                existingSeguro.Nombres = UpdItem.Nombres;
                existingSeguro.Apellidos = UpdItem.Apellidos;
                existingSeguro.CiAfiliado = UpdItem.CiAfiliado;
                existingSeguro.Telefono = UpdItem.Telefono;
                existingSeguro.Tipo = UpdItem.Tipo;
                existingSeguro.Estado = UpdItem.Estado;
                _context.SaveChanges();
            }
        }


        public void DeleteSegurovidumByCedula(string CiBeneficiario, string CiAfiliado)
        {
            var seguro = _context.Segurovida
                .FirstOrDefault(s => s.CiBeneficiario == CiBeneficiario && s.CiAfiliado == CiAfiliado);

            if (seguro != null)
            {
                _context.Segurovida.Remove(seguro);
                _context.SaveChanges();
            }
        }

        // Paginado
        public async Task<Paginado.PagedResult<Segurovidum>> GetSegurovidumPaginados(
            int pagina,
            int pageSize,
            string? CiBeneficiario = null,
            string? CiAfiliado = null)
        {
            var query = _context.Segurovida.AsQueryable();
            if (!string.IsNullOrEmpty(CiBeneficiario))
            {
                query = query.Where(s => s.CiBeneficiario.Contains(CiBeneficiario));
            }
            if (!string.IsNullOrEmpty(CiAfiliado))
            {
                query = query.Where(s => s.CiAfiliado.Contains(CiAfiliado));
            }
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Segurovidum>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }

        // Paginado por cedula afiliado
        public async Task<Paginado.PagedResult<Segurovidum>> GetSegurovidumPaginadosByCedulaAfiliado(
            int pagina,
            int pageSize,
            string CiAfiliado)
        {
            var query = _context.Segurovida
                .Where(s => s.CiAfiliado == CiAfiliado)
                .AsQueryable();
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Segurovidum>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }

    }
}
