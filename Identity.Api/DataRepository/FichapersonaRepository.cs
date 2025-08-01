using Identity.Api.DTO;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class FichapersonaRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public FichapersonaRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<FichapersonalDTO> GetFichaPersonalInfoAll()
        {
            return _context.Fichapersonals
                .Where(x => x.Estado == "a")
                .Select(f => new FichapersonalDTO
                {
                    Cedula = f.Cedula,
                    Nombre = f.Nombre,
                    Apellidos = f.Apellidos,
                    Telefono = f.Telefono,
                    Celular = f.Celular,
                    Correo = f.Correo,
                    Fechanacimiento = f.Fechanacimiento,
                    Fechaingreso = f.Fechaingreso,
                    Domicilio = f.Domicilio,
                    Referencia = f.Referencia,
                    Estado = f.Estado,
                    Estadoservicio = f.Estadoservicio,
                    Cuotaf = f.Cuotaf,
                })
                .ToList();
        }

        public FichapersonalDTO GetFichaPersonalById(string cedula)
        {
            var ficha = _context.Fichapersonals.FirstOrDefault(x => x.Cedula == cedula && x.Estado == "a");
            if (ficha == null) return null;
            return new FichapersonalDTO
            {
                Cedula = ficha.Cedula,
                Nombre = ficha.Nombre,
                Apellidos = ficha.Apellidos,
                Telefono = ficha.Telefono,
                Celular = ficha.Celular,
                Correo = ficha.Correo,
                Fechanacimiento = ficha.Fechanacimiento,
                Fechaingreso = ficha.Fechaingreso,
                Domicilio = ficha.Domicilio,
                Referencia = ficha.Referencia,
                Estado = ficha.Estado,
                Estadoservicio = ficha.Estadoservicio,
                Cuotaf = ficha.Cuotaf
            };
        }

        public void InsertFichaPersonal(FichapersonalDTO nueva)
        {
            var ficha = new Fichapersonal
            {
                Cedula = nueva.Cedula,
                Nombre = nueva.Nombre,
                Apellidos = nueva.Apellidos,
                Telefono = nueva.Telefono,
                Celular = nueva.Celular,
                Correo = nueva.Correo,
                Fechanacimiento = nueva.Fechanacimiento,
                Fechaingreso = nueva.Fechaingreso,
                Domicilio = nueva.Domicilio,
                Referencia = nueva.Referencia,
                Estadoservicio = nueva.Estadoservicio,
                Cuotaf = nueva.Cuotaf,
                Estado = "a" // Asignar estado activo por defecto
            };
            _context.Fichapersonals.Add(ficha);
            _context.SaveChanges();
        }

        public void UpdateFichaPersonal(FichapersonalDTO actualizada)
        {
            var ficha = _context.Fichapersonals.FirstOrDefault(x => x.Cedula == actualizada.Cedula);
            if (ficha != null)
            {
                ficha.Cedula = actualizada.Cedula;
                ficha.Nombre = actualizada.Nombre;
                ficha.Apellidos = actualizada.Apellidos;
                ficha.Telefono = actualizada.Telefono;
                ficha.Celular = actualizada.Celular;
                ficha.Correo = actualizada.Correo;
                ficha.Fechanacimiento = actualizada.Fechanacimiento;
                ficha.Fechaingreso = actualizada.Fechaingreso;
                ficha.Domicilio = actualizada.Domicilio;
                ficha.Referencia = actualizada.Referencia;
                ficha.Estadoservicio = actualizada.Estadoservicio;
                ficha.Cuotaf = actualizada.Cuotaf;
                ficha.Estado = actualizada.Estado; // Actualizar estado
                _context.SaveChanges();
            }
        }

        public void DeleteFichaPersonalById(string cedula)
        {
            var ficha = _context.Fichapersonals.FirstOrDefault(x => x.Cedula == cedula);
            if (ficha != null)
            {
                ficha.Estado = "i"; // Cambiar estado a inactivo
                _context.SaveChanges();
            }
        }

        // Paginado
        public async Task<PagedResult<FichapersonalDTO>> GetFichaPersonalPaginados(
            int pagina,
            int pageSize,
            string? filtro = null,
            string? estado = null)
        {
            var query = _context.Fichapersonals.AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(f => f.Nombre.Contains(filtro) || f.Apellidos.Contains(filtro));
            }


            if (string.IsNullOrWhiteSpace(estado))
            {
                estado = "a";
            }

            query = query.Where(f => f.Estado == estado);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FichapersonalDTO
                {
                    Cedula = f.Cedula,
                    Nombre = f.Nombre,
                    Apellidos = f.Apellidos,
                    Telefono = f.Telefono,
                    Celular = f.Celular,
                    Correo = f.Correo,
                    Fechanacimiento = f.Fechanacimiento,
                    Fechaingreso = f.Fechaingreso,
                    Domicilio = f.Domicilio,
                    Referencia = f.Referencia,
                    Estado = f.Estado,
                    Estadoservicio = f.Estadoservicio,
                    Cuotaf = f.Cuotaf,
                })
                .ToListAsync();
            return new PagedResult<FichapersonalDTO>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }

    }
}
