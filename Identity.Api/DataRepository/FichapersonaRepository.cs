using FluentFTP;
using Identity.Api.DTO;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;
using System.Net;

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
                    Fknacionalidad = f.Fknacionalidad,
                    Fktipolicencia = f.Fktipolicencia,
                    Fkcargo = f.Fkcargo,
                    Fkniveleducacion = f.Fkniveleducacion,
                    Fkunidad = f.Fkunidad,
                    Fkdpuesto = f.Fkdpuesto,
                })
                .ToList();
        }

        public FichapersonalDTO GetFichaPersonalById(string cedula)
        {
            var f = _context.Fichapersonals.FirstOrDefault(x => x.Cedula == cedula && x.Estado == "a");
            if (f == null) return null;
            return new FichapersonalDTO
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
                Fknacionalidad = f.Fknacionalidad,
                Fktipolicencia = f.Fktipolicencia,
                Fkcargo = f.Fkcargo,
                Fkniveleducacion = f.Fkniveleducacion,
                Fkunidad = f.Fkunidad,
                Fkdpuesto = f.Fkdpuesto,
            };
        }

        public FichapersonalDTO GetFichaPersonalByCorreo(string correo)
        {
            //trae todo
            var f = _context.Fichapersonals
            .FirstOrDefault(x => x.Correo.ToLower() == correo.ToLower());
            //var f = _context.Fichapersonals
            //.FirstOrDefault(x => x.Correo.ToLower() == correo.ToLower() && x.Estado == "a");
            if (f == null) return null;
            return new FichapersonalDTO
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
                Fknacionalidad = f.Fknacionalidad,
                Fktipolicencia = f.Fktipolicencia,
                Fkcargo = f.Fkcargo,
                Fkniveleducacion = f.Fkniveleducacion,
                Fkunidad = f.Fkunidad,
                Fkdpuesto = f.Fkdpuesto,
            };
        }

        public void InsertFichaPersonal(FichapersonalDTO f)
        {
            // Validaciones previas

            if (string.IsNullOrWhiteSpace(f.Cedula))
                throw new Exception("La cédula es obligatoria.");

            if (_context.Fichapersonals.Any(x => x.Cedula == f.Cedula))
                throw new Exception("Ya existe una ficha con esa cédula.");

            if (string.IsNullOrWhiteSpace(f.Nombre))
                throw new Exception("El nombre es obligatorio.");

            if (f.Fkcargo == 0)
                throw new Exception("Debe seleccionar un cargo.");

            if (f.Fktipolicencia == 0)
                throw new Exception("Debe seleccionar un tipo de licencia.");

            if (f.Fknacionalidad == 0)
                throw new Exception("Debe seleccionar una nacionalidad.");

            // Crear entidad
            var ficha = new Fichapersonal
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
                Estado = f.Estado ?? "a", // Por defecto activo
                Estadoservicio = f.Estadoservicio,
                Cuotaf = f.Cuotaf,
                Fknacionalidad = f.Fknacionalidad,
                Fktipolicencia = f.Fktipolicencia,
                Fkcargo = f.Fkcargo,
                Fkniveleducacion = f.Fkniveleducacion,
                Fkunidad = f.Fkunidad,
                Fkdpuesto = f.Fkdpuesto,
            };

            try
            {
                _context.Fichapersonals.Add(ficha);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Mostrar mensaje más específico de la InnerException
                throw new Exception("Error al crear: " + ex.InnerException?.Message ?? ex.Message);
            }
        }


        public void UpdateFichaPersonal(FichapersonalDTO f)
        {
            var ficha = _context.Fichapersonals.FirstOrDefault(x => x.Cedula == f.Cedula);
            if (ficha != null)
            {
                ficha.Cedula = f.Cedula;
                ficha.Nombre = f.Nombre;
                ficha.Apellidos = f.Apellidos;
                ficha.Telefono = f.Telefono;
                ficha.Celular = f.Celular;
                ficha.Correo = f.Correo;
                ficha.Fechanacimiento = f.Fechanacimiento;
                ficha.Fechaingreso = f.Fechaingreso;
                ficha.Domicilio = f.Domicilio;
                ficha.Referencia = f.Referencia;
                ficha.Estado = f.Estado;
                ficha.Estadoservicio = f.Estadoservicio;
                ficha.Cuotaf = f.Cuotaf;
                ficha.Fknacionalidad = f.Fknacionalidad;
                ficha.Fktipolicencia = f.Fktipolicencia;
                ficha.Fkcargo = f.Fkcargo;
                ficha.Fkniveleducacion = f.Fkniveleducacion;
                ficha.Fkunidad = f.Fkunidad;
                ficha.Fkdpuesto = f.Fkdpuesto;
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
                query = query.Where(f =>
                    f.Cedula.Contains(filtro) ||
                    f.Nombre.Contains(filtro) ||
                    f.Apellidos.Contains(filtro));
            }

            if (string.IsNullOrWhiteSpace(estado))
            {
                estado = "a";
            }

            query = query.Where(f => f.Estado == estado);

            var totalItems = await query.CountAsync();

            // Solo traemos los datos de la ficha, sin validar documentos
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
                    Fknacionalidad = f.Fknacionalidad,
                    Fktipolicencia = f.Fktipolicencia,
                    Fkcargo = f.Fkcargo,
                    Fkniveleducacion = f.Fkniveleducacion,
                    Fkunidad = f.Fkunidad,
                    Fkdpuesto = f.Fkdpuesto,

                    // Inicializamos como null para que el frontend lo calcule
                    
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
