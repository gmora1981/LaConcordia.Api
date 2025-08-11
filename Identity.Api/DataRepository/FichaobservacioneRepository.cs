using Identity.Api.DTO;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class FichaobservacioneRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public FichaobservacioneRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Fichaobservacione> GetFichaObservacioneInfoAll()
        {
            return _context.Fichaobservaciones.ToList();
        }

        public List<FichaobservacioneDTO> GetFichaObservacioneByCedula(string cedula)
        {
            using var context = new DbAa5796GmoraContext();

            return context.Fichaobservaciones
                .Where(f => f.Fkcedula == cedula)
                .Select(s => new FichaobservacioneDTO
                {
                    Idfichaobs = s.Idfichaobs,
                    Fkcedula = s.Fkcedula,
                    Fkunidad = s.Fkunidad,
                    Fecha = s.Fecha,
                    Motivo = s.Motivo
                })
                .ToList();
        }


        public FichaobservacioneDTO GetFichaObservacioneById(int id)
        {
            using var context = new DbAa5796GmoraContext();
            var s = context.Fichaobservaciones
                .FirstOrDefault(f => f.Idfichaobs == id);
            if (s == null) return null;
            return new FichaobservacioneDTO
            {
                Idfichaobs = s.Idfichaobs,
                Fkcedula = s.Fkcedula,
                Fkunidad = s.Fkunidad,
                Fecha = s.Fecha,
                Motivo = s.Motivo
            };
        }

        public void InsertFichaObservacione(FichaobservacioneDTO New)
        {
            using var context = new DbAa5796GmoraContext();
            var newFicha = new Fichaobservacione
            {
                Fkcedula = New.Fkcedula,
                Fkunidad = New.Fkunidad,
                Fecha = New.Fecha,
                Motivo = New.Motivo
            };
            context.Fichaobservaciones.Add(newFicha);
            context.SaveChanges();
        }

        public void UpdateFichaObservacione(FichaobservacioneDTO fichaObservacione)
        {
            using var context = new DbAa5796GmoraContext();
            var existingFicha = context.Fichaobservaciones
                .FirstOrDefault(f => f.Idfichaobs == fichaObservacione.Idfichaobs);
            if (existingFicha != null)
            {
                existingFicha.Fkcedula = fichaObservacione.Fkcedula;
                existingFicha.Fkunidad = fichaObservacione.Fkunidad;
                existingFicha.Fecha = fichaObservacione.Fecha;
                existingFicha.Motivo = fichaObservacione.Motivo;
                context.SaveChanges();
            }
        }

        public void DeleteFichaObservacioneByCedula(int cedula)
        {
            using var context = new DbAa5796GmoraContext();
            var fichaToDelete = context.Fichaobservaciones
                .FirstOrDefault(f => f.Idfichaobs == cedula);
            if (fichaToDelete != null)
            {
                context.Fichaobservaciones.Remove(fichaToDelete);
                context.SaveChanges();
            }
        }

        // Paginado
        public async Task<PagedResult<Fichaobservacione>> GetFichaObservacionePaginados(
            int pagina,
            int pageSize,
            string? unidad = null,
            string? cedula = null)
        {
            var query = _context.Fichaobservaciones.AsQueryable();
            if (!string.IsNullOrEmpty(unidad))
            {
                query = query.Where(f => f.Fkunidad == unidad);
            }
            if (!string.IsNullOrEmpty(cedula))
            {
                query = query.Where(f => f.Fkcedula == cedula);
            }
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Fichaobservacione>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }

        // Paginado por ID
        public async Task<PagedResult<Fichaobservacione>> GetFichaObservacionePaginadosByCedula(
            int pagina,
            int pageSize,
            string Fkcedula)
        {
            var query = _context.Fichaobservaciones.AsQueryable();

            // Filtrar por Idfichaobs
            query = query.Where(f => f.Fkcedula == Fkcedula);

            // Ordenar por fecha descendente
            query = query.OrderByDescending(f => f.Fecha);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Fichaobservacione>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }


    }
}
