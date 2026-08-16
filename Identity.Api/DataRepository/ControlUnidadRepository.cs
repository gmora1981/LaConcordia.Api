using Identity.Api.DTO;
using Identity.Api.Reporteria;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class ControlUnidadRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public ControlUnidadRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public List<UnidadServicioDTO> GetFichaPersonalPorServicio(string estadoServicio)
        {
            using var context = new DbAa5796GmoraContext();

            return context.Fichapersonals
                .Where(f => f.Estado == "a" && f.Estadoservicio == estadoServicio && f.Fkunidad != null && f.Fkunidad != "")
                .Select(f => new UnidadServicioDTO
                {
                    Fkunidad = f.Fkunidad!,
                    Cedula = f.Cedula,
                    Nombre = f.Nombre,
                    Apellidos = f.Apellidos
                })
                .OrderBy(f => f.Fkunidad)
                .ToList();
        }

        public void MoverUnidad(MoverUnidadRequestDTO request, string monitora)
        {
            var ficha = _context.Fichapersonals.FirstOrDefault(f => f.Cedula == request.Cedula);
            if (ficha == null)
                throw new Exception($"No se encontró la ficha personal con cédula '{request.Cedula}'. No se pudo actualizar el estado de la unidad.");

            var nombreCompleto = $"{ficha.Apellidos} {ficha.Nombre}".Trim();

            _context.Controlunidades.Add(new Controlunidade
            {
                Fecharegistro = DateTime.Now,
                Unidad = request.Unidad,
                Estado = request.Direccion,
                Turno = request.Turno,
                Monitora = monitora,
                Monitorasig = "",
                Ciconductor = request.Cedula,
                Conductor = nombreCompleto
            });

            ficha.Estadoservicio = request.Direccion == "INGRESO" ? "a" : "p";

            _context.SaveChanges();
        }

        // Historial de ingresos/salidas de un dia (opcionalmente filtrado por turno), para el reporte.
        public List<ControlUnidadMovimientoDTO> GetMovimientos(DateTime fecha, string? turno)
        {
            using var context = new DbAa5796GmoraContext();

            var query = context.Controlunidades.Where(c => c.Fecharegistro.Date == fecha.Date);

            if (!string.IsNullOrEmpty(turno))
                query = query.Where(c => c.Turno == turno);

            return query
                .OrderBy(c => c.Fecharegistro)
                .Select(c => new ControlUnidadMovimientoDTO
                {
                    Fecharegistro = c.Fecharegistro,
                    Turno = c.Turno,
                    Unidad = c.Unidad,
                    Ciconductor = c.Ciconductor,
                    Conductor = c.Conductor,
                    Estado = c.Estado
                })
                .ToList();
        }

        // Operadoras/monitoras que ya tienen movimientos registrados, para el combo del filtro
        // (no hay un rol/tabla dedicada; se listan los valores distintos que realmente existen).
        public List<string> GetMonitorasDisponibles()
        {
            using var context = new DbAa5796GmoraContext();

            return context.Controlunidades
                .Where(c => c.Monitora != null && c.Monitora != "")
                .Select(c => c.Monitora!)
                .Distinct()
                .OrderBy(m => m)
                .ToList();
        }

        // Unidades que ya tienen movimientos registrados, para el combo del filtro por unidad.
        public List<string> GetUnidadesConMovimientos()
        {
            using var context = new DbAa5796GmoraContext();

            return context.Controlunidades
                .Select(c => c.Unidad)
                .Distinct()
                .OrderBy(u => u)
                .ToList();
        }

        // "Reporte de Ingreso y Salida" (FrmReporteOperadora / RptOperadorasIngresoSalidaUni, y
        // FrmReporteUnidad / RptUnidadesIngresoSalida, del escritorio): movimientos dentro de un
        // rango de fechas, opcionalmente filtrados por operadora/monitora y/o por unidad.
        public List<ControlUnidadMovimientoDTO> GetMovimientosPorRango(DateTime desde, DateTime hasta, string? monitora, string? unidad = null)
        {
            using var context = new DbAa5796GmoraContext();

            var hastaExclusivo = hasta.Date.AddDays(1);
            var query = context.Controlunidades
                .Where(c => c.Fecharegistro >= desde.Date && c.Fecharegistro < hastaExclusivo);

            if (!string.IsNullOrEmpty(monitora))
                query = query.Where(c => c.Monitora == monitora);

            if (!string.IsNullOrEmpty(unidad))
                query = query.Where(c => c.Unidad == unidad);

            return query
                .OrderBy(c => c.Fecharegistro)
                .Select(c => new ControlUnidadMovimientoDTO
                {
                    Fecharegistro = c.Fecharegistro,
                    Turno = c.Turno,
                    Unidad = c.Unidad,
                    Ciconductor = c.Ciconductor,
                    Conductor = c.Conductor,
                    Estado = c.Estado,
                    Monitora = c.Monitora
                })
                .ToList();
        }

        public byte[] ExportarReporteIngresoSalidaPdf(DateTime desde, DateTime hasta, string? monitora, string? unidad, string usuario)
        {
            var movimientos = GetMovimientosPorRango(desde, hasta, monitora, unidad);
            return ReporteIngresoSalidaPdfGenerator.GenerarPdf(movimientos, desde, hasta, monitora, unidad, usuario);
        }
    }
}
