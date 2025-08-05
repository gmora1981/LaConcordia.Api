using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public void InsertFichaPersonal(FichapersonalDTO f)
        {
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
                Estado = f.Estado,
                Estadoservicio = f.Estadoservicio,
                Cuotaf = f.Cuotaf,
                Fknacionalidad = f.Fknacionalidad,
                Fktipolicencia = f.Fktipolicencia,
                Fkcargo = f.Fkcargo,
                Fkniveleducacion = f.Fkniveleducacion,
                Fkunidad = f.Fkunidad,
                Fkdpuesto = f.Fkdpuesto,
                //Estado = "a" // Asignar estado activo por defecto
            };
            _context.Fichapersonals.Add(ficha);
            _context.SaveChanges();
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
