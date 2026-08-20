using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class SolicitudCarreraRepository
    {
        public void CrearSolicitud(CrearSolicitudCarreraRequestDTO request, string ruc)
        {
            using var context = new DbAa5796GmoraContext();

            context.Solicitudcarreras.Add(new Solicitudcarrera
            {
                Ruc = ruc,
                Celular = request.Celular,
                Empleado = request.Empleado,
                Origenlat = request.Origenlat,
                Origenlog = request.Origenlog,
                Destinolat = request.Destinolat,
                Destinolog = request.Destinolog,
                Observacion = request.Observacion,
                Fechasolicitud = DateTime.Now,
                Estado = "PENDIENTE"
            });

            context.SaveChanges();
        }

        public List<SolicitudCarreraDTO> GetMisSolicitudes(string ruc)
        {
            using var context = new DbAa5796GmoraContext();

            return context.Solicitudcarreras
                .Where(s => s.Ruc == ruc)
                .OrderByDescending(s => s.Fechasolicitud)
                .Select(x => new SolicitudCarreraDTO
                {
                    Idsolicitud = x.Idsolicitud,
                    Ruc = x.Ruc,
                    Celular = x.Celular,
                    Empleado = x.Empleado,
                    Origenlat = x.Origenlat,
                    Origenlog = x.Origenlog,
                    Destinolat = x.Destinolat,
                    Destinolog = x.Destinolog,
                    Observacion = x.Observacion,
                    Fechasolicitud = x.Fechasolicitud,
                    Estado = x.Estado
                })
                .ToList();
        }

        // Bandeja del despachador: solo las pendientes, con la Razon Social ya resuelta.
        public List<SolicitudCarreraDTO> GetSolicitudesPendientes()
        {
            using var context = new DbAa5796GmoraContext();

            var pendientes = context.Solicitudcarreras
                .Where(s => s.Estado == "PENDIENTE")
                .OrderBy(s => s.Fechasolicitud)
                .ToList();

            var rucs = pendientes.Select(s => s.Ruc).Distinct().ToList();
            var empresas = context.Empresas
                .Where(e => rucs.Contains(e.Ruc))
                .ToDictionary(e => e.Ruc, e => e.Razonsocial);

            return pendientes.Select(x => new SolicitudCarreraDTO
            {
                Idsolicitud = x.Idsolicitud,
                Ruc = x.Ruc,
                RazonSocial = empresas.TryGetValue(x.Ruc, out var razonSocial) ? razonSocial : null,
                Celular = x.Celular,
                Empleado = x.Empleado,
                Origenlat = x.Origenlat,
                Origenlog = x.Origenlog,
                Destinolat = x.Destinolat,
                Destinolog = x.Destinolog,
                Observacion = x.Observacion,
                Fechasolicitud = x.Fechasolicitud,
                Estado = x.Estado
            }).ToList();
        }

        public void MarcarConvertida(int idsolicitud, string usuario)
        {
            using var context = new DbAa5796GmoraContext();

            var solicitud = context.Solicitudcarreras.FirstOrDefault(s => s.Idsolicitud == idsolicitud);
            if (solicitud == null)
                throw new InvalidOperationException("No se encontró la solicitud.");

            solicitud.Estado = "CONVERTIDA";
            solicitud.Fechaconversion = DateTime.Now;
            solicitud.Usuarioconversion = usuario;

            context.SaveChanges();
        }
    }
}
