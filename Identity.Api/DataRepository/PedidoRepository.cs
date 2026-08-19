using Identity.Api.DTO;
using Identity.Api.Paginado;
using Identity.Api.Reporteria;
using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class PedidoRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public PedidoRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public IEnumerable<Pedido> GetPedidoInfoAll()
        {
            return _context.Pedidos.ToList();
        }

        public void InsertPedido(Pedido nuevo)
        {
            _context.Pedidos.Add(nuevo);
            _context.SaveChanges();
        }

        public void UpdatePedido(Pedido actualizado)
        {
            var registrado = _context.Pedidos.FirstOrDefault(p =>
                p.Celular == actualizado.Celular &&
                p.Origenlat == actualizado.Origenlat &&
                p.Origenlog == actualizado.Origenlog &&
                p.Destinolat == actualizado.Destinolat &&
                p.Destinolog == actualizado.Destinolog &&
                p.Fecharegistro == actualizado.Fecharegistro);

            if (registrado != null)
            {
                registrado.Tiempodemora = actualizado.Tiempodemora;
                registrado.Ruc = actualizado.Ruc;
                registrado.Usuario = actualizado.Usuario;
                registrado.Base = actualizado.Base;
                registrado.Unidad = actualizado.Unidad;
                registrado.Ciconductor = actualizado.Ciconductor;
                registrado.Conductor = actualizado.Conductor;
                registrado.Unidadsiguiente = actualizado.Unidadsiguiente;
                registrado.Ciconductorsiguiente = actualizado.Ciconductorsiguiente;
                registrado.Conductorsiguiente = actualizado.Conductorsiguiente;
                registrado.Precio = actualizado.Precio;
                registrado.Km = actualizado.Km;
                registrado.Numvoucher = actualizado.Numvoucher;
                registrado.Valija = actualizado.Valija;
                registrado.Empleado = actualizado.Empleado;
                registrado.Recorrido = actualizado.Recorrido;
                registrado.Estado = actualizado.Estado;
                registrado.Autorizado = actualizado.Autorizado;

                _context.SaveChanges();
            }
        }

        //paginado
        public async Task<PagedResult<PedidoDTO>> GetPedidoPaginados(
            int pagina,
            int pageSize,
            string? celular = null,
            string? unidad = null,
            string? estado = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            var query = _context.Pedidos.AsQueryable();

            if (!string.IsNullOrEmpty(celular))
                query = query.Where(x => x.Celular.Contains(celular));

            if (!string.IsNullOrEmpty(unidad))
                query = query.Where(x => x.Unidad != null && x.Unidad.Contains(unidad));

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(x => x.Estado == estado);

            if (fechaDesde.HasValue)
                query = query.Where(x => x.Fecharegistro >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(x => x.Fecharegistro <= fechaHasta.Value);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Fecharegistro)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PedidoDTO
                {
                    Celular = x.Celular,
                    Origenlat = x.Origenlat,
                    Origenlog = x.Origenlog,
                    Destinolat = x.Destinolat,
                    Destinolog = x.Destinolog,
                    Tiempodemora = x.Tiempodemora,
                    Ruc = x.Ruc,
                    Fecharegistro = x.Fecharegistro,
                    Usuario = x.Usuario,
                    Base = x.Base,
                    Unidad = x.Unidad,
                    Ciconductor = x.Ciconductor,
                    Conductor = x.Conductor,
                    Unidadsiguiente = x.Unidadsiguiente,
                    Ciconductorsiguiente = x.Ciconductorsiguiente,
                    Conductorsiguiente = x.Conductorsiguiente,
                    Precio = x.Precio,
                    Km = x.Km,
                    Numvoucher = x.Numvoucher,
                    Valija = x.Valija,
                    Empleado = x.Empleado,
                    Recorrido = x.Recorrido,
                    Estado = x.Estado,
                    Autorizado = x.Autorizado
                })
                .ToListAsync();

            return new PagedResult<PedidoDTO>
            {
                Items = items,
                TotalItems = totalItems,
                Page = pagina,
                PageSize = pageSize
            };
        }

        public ConductorInfoDTO? GetConductorPorUnidad(string unidad)
        {
            using var context = new DbAa5796GmoraContext();

            var ficha = context.Fichapersonals
                .FirstOrDefault(f => f.Fkunidad == unidad && f.Estado == "a");

            if (ficha == null) return null;

            return new ConductorInfoDTO
            {
                Cedula = ficha.Cedula,
                NombreCompleto = $"{ficha.Apellidos} {ficha.Nombre}".Trim()
            };
        }

        // Misma coordenada placeholder que usa el frontend para "sin destino definido todavia".
        private const decimal LatSinDestino = -0.380284m;
        private const decimal LngSinDestino = -91.5448445m;

        public List<PedidoDTO> GetPedidosConDestinoPendiente()
        {
            using var context = new DbAa5796GmoraContext();

            return context.Pedidos
                .Where(p => p.Destinolat == LatSinDestino && p.Destinolog == LngSinDestino)
                .OrderByDescending(p => p.Fecharegistro)
                .Select(x => new PedidoDTO
                {
                    Celular = x.Celular,
                    Origenlat = x.Origenlat,
                    Origenlog = x.Origenlog,
                    Destinolat = x.Destinolat,
                    Destinolog = x.Destinolog,
                    Tiempodemora = x.Tiempodemora,
                    Ruc = x.Ruc,
                    Fecharegistro = x.Fecharegistro,
                    Usuario = x.Usuario,
                    Base = x.Base,
                    Unidad = x.Unidad,
                    Ciconductor = x.Ciconductor,
                    Conductor = x.Conductor,
                    Unidadsiguiente = x.Unidadsiguiente,
                    Ciconductorsiguiente = x.Ciconductorsiguiente,
                    Conductorsiguiente = x.Conductorsiguiente,
                    Precio = x.Precio,
                    Km = x.Km,
                    Numvoucher = x.Numvoucher,
                    Valija = x.Valija,
                    Empleado = x.Empleado,
                    Recorrido = x.Recorrido,
                    Estado = x.Estado,
                    Autorizado = x.Autorizado
                })
                .ToList();
        }

        // Guarda (si no existe ya) la direccion resuelta para unas coordenadas, para que
        // luego Orden de Pago pueda mostrar el Punto de Partida/Final en texto. Requiere que
        // el pasajero ya exista (Direccion.Celular es llave foranea hacia Pasajero); si aun
        // no existe, no hace nada en vez de fallar.
        public void GuardarDireccion(string celular, decimal lat, decimal lng, string? calle)
        {
            using var context = new DbAa5796GmoraContext();

            // Direccion.Celular es llave foranea hacia Pasajero. Se crea un pasajero minimo
            // (solo con el celular) si todavia no existe, en vez de exigir que el despachador
            // haya guardado antes manualmente los datos completos del pasajero.
            var existePasajero = context.Pasajeros.Any(p => p.Celular == celular);
            if (!existePasajero)
            {
                context.Pasajeros.Add(new Pasajero { Celular = celular });
                context.SaveChanges();
            }

            var yaExiste = context.Direccions.Any(d =>
                d.Celular == celular && d.Latitud == lat && d.Longitud == lng);
            if (yaExiste) return;

            context.Direccions.Add(new Direccion
            {
                Celular = celular,
                Latitud = lat,
                Longitud = lng,
                Calle = calle
            });

            context.SaveChanges();
        }

        public PrecioKmDTO? GetPrecioKmHistorico(string celular, decimal origenLat, decimal origenLog, decimal destinoLat, decimal destinoLog)
        {
            using var context = new DbAa5796GmoraContext();

            var anterior = context.Pedidos
                .Where(p => p.Celular == celular
                    && p.Origenlat == origenLat && p.Origenlog == origenLog
                    && p.Destinolat == destinoLat && p.Destinolog == destinoLog
                    && p.Precio != null && p.Precio > 0)
                .OrderByDescending(p => p.Fecharegistro)
                .FirstOrDefault();

            if (anterior == null) return null;

            return new PrecioKmDTO
            {
                Precio = anterior.Precio ?? 0,
                Km = anterior.Km ?? 0
            };
        }

        // Dashboard "Total de Ingresos de Carreras Asignadas": cantidad de pedidos registrados
        // por cada usuario (despachador) dentro de [desde, hasta] (hasta incluye el dia completo).
        public List<PedidosPorUsuarioDTO> GetCantidadPedidosPorUsuario(DateTime desde, DateTime hasta)
        {
            using var context = new DbAa5796GmoraContext();

            var hastaExclusivo = hasta.Date.AddDays(1);

            return context.Pedidos
                .Where(p => p.Fecharegistro >= desde.Date && p.Fecharegistro < hastaExclusivo
                    && p.Usuario != null && p.Usuario != "")
                .GroupBy(p => p.Usuario!)
                .Select(g => new PedidosPorUsuarioDTO
                {
                    Usuario = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToList();
        }

        // Dashboard "Top 10 de las Unidades con Mas Carreras": cantidad de pedidos por unidad
        // dentro de [desde, hasta] (hasta incluye el dia completo), las 10 con mas carreras.
        public List<PedidosPorUnidadDTO> GetTopUnidadesConMasCarreras(DateTime desde, DateTime hasta)
        {
            using var context = new DbAa5796GmoraContext();

            var hastaExclusivo = hasta.Date.AddDays(1);

            return context.Pedidos
                .Where(p => p.Fecharegistro >= desde.Date && p.Fecharegistro < hastaExclusivo
                    && p.Unidad != null && p.Unidad != "")
                .GroupBy(p => p.Unidad!)
                .Select(g => new PedidosPorUnidadDTO
                {
                    Unidad = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(10)
                .ToList();
        }

        // Usuarios (despachadores) que ya tienen pedidos registrados, para el combo del filtro
        // (no hay un rol/tabla dedicada; se listan los valores distintos que realmente existen).
        public List<string> GetUsuariosDisponibles()
        {
            using var context = new DbAa5796GmoraContext();

            return context.Pedidos
                .Where(p => p.Usuario != null && p.Usuario != "")
                .Select(p => p.Usuario!)
                .Distinct()
                .OrderBy(u => u)
                .ToList();
        }

        // Unidades que ya tienen pedidos registrados, para el combo del filtro por unidad
        // (equivalente a FrmReporteUnidad -> "Detalle de Pedido").
        public List<string> GetUnidadesConPedidos()
        {
            using var context = new DbAa5796GmoraContext();

            return context.Pedidos
                .Where(p => p.Unidad != null && p.Unidad != "")
                .Select(p => p.Unidad!)
                .Distinct()
                .OrderBy(u => u)
                .ToList();
        }

        // "Reporte de Solicitud de Carrera": detalle de pedidos dentro de [desde, hasta],
        // filtrado opcionalmente por usuario/operadora (FrmReporteOperadora) y/o por unidad
        // (FrmReporteUnidad -> "Detalle de Pedido"), ambos filtros independientes y combinables.
        // Las direcciones se resuelven desde la tabla Direccion ya guardada, en vez de volver a
        // geocodificar cada fila.
        public List<PedidoOperadoraDTO> GetPedidosPorOperadora(string? usuario, DateTime desde, DateTime hasta, string? unidad = null)
        {
            using var context = new DbAa5796GmoraContext();

            var hastaExclusivo = hasta.Date.AddDays(1);
            var query = context.Pedidos
                .Where(p => p.Fecharegistro >= desde.Date && p.Fecharegistro < hastaExclusivo);

            if (!string.IsNullOrEmpty(usuario))
                query = query.Where(p => p.Usuario == usuario);

            if (!string.IsNullOrEmpty(unidad))
                query = query.Where(p => p.Unidad == unidad);

            var pedidos = query.OrderBy(p => p.Fecharegistro).ToList();

            var celulares = pedidos.Select(p => p.Celular).Distinct().ToList();
            var direcciones = context.Direccions
                .Where(d => celulares.Contains(d.Celular))
                .ToList();

            string? BuscarCalle(string celular, decimal lat, decimal lng)
            {
                var d = direcciones.FirstOrDefault(x => x.Celular == celular && x.Latitud == lat && x.Longitud == lng);
                if (d == null) return null;
                return $"{d.Calle} {d.Numero} {d.Referencia}".Trim();
            }

            return pedidos.Select(p => new PedidoOperadoraDTO
            {
                Fecharegistro = p.Fecharegistro,
                CalleOrigen = BuscarCalle(p.Celular, p.Origenlat, p.Origenlog),
                CalleDestino = BuscarCalle(p.Celular, p.Destinolat, p.Destinolog),
                Usuario = p.Usuario,
                Unidad = p.Unidad,
                Precio = p.Precio
            }).ToList();
        }

        public byte[] ExportarReporteSolicitudCarreraPdf(string? usuario, DateTime desde, DateTime hasta, string? unidad, string usuarioLogueado)
        {
            var lista = GetPedidosPorOperadora(usuario, desde, hasta, unidad);
            var conductor = !string.IsNullOrEmpty(unidad) ? GetConductorPorUnidad(unidad)?.NombreCompleto : null;
            return ReporteSolicitudCarreraPdfGenerator.GenerarPdf(lista, usuario, unidad, conductor, desde, hasta, usuarioLogueado);
        }

        // ===== App del conductor (Taxista) =====

        public InfoConductorDTO? GetInfoConductorPorCedula(string cedula)
        {
            using var context = new DbAa5796GmoraContext();

            var ficha = context.Fichapersonals.FirstOrDefault(f => f.Cedula == cedula && f.Estado == "a");
            if (ficha == null) return null;

            return new InfoConductorDTO
            {
                Cedula = ficha.Cedula,
                NombreCompleto = $"{ficha.Apellidos} {ficha.Nombre}".Trim(),
                Unidad = ficha.Fkunidad
            };
        }

        // "Mis Carreras" del conductor: pedidos asignados a su unidad, sin paginar (la lista
        // diaria de un conductor es corta). Mismo filtro Unidad/Estado que GetPedidoPaginados.
        public List<PedidoDTO> GetCarrerasAsignadas(string unidad, string? estado = null)
        {
            using var context = new DbAa5796GmoraContext();

            var query = context.Pedidos.Where(p => p.Unidad == unidad);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(p => p.Estado == estado);

            var pedidos = query.OrderByDescending(p => p.Fecharegistro).ToList();

            // Igual que GetPedidosPorOperadora: se resuelve la calle desde la tabla Direccion
            // ya guardada, en vez de mostrarle al conductor coordenadas crudas.
            var celulares = pedidos.Select(p => p.Celular).Distinct().ToList();
            var direcciones = context.Direccions
                .Where(d => celulares.Contains(d.Celular))
                .ToList();

            string? BuscarCalle(string celular, decimal lat, decimal lng)
            {
                var d = direcciones.FirstOrDefault(x => x.Celular == celular && x.Latitud == lat && x.Longitud == lng);
                if (d == null) return null;
                return $"{d.Calle} {d.Numero} {d.Referencia}".Trim();
            }

            return pedidos
                .Select(x => new PedidoDTO
                {
                    Celular = x.Celular,
                    Origenlat = x.Origenlat,
                    Origenlog = x.Origenlog,
                    Destinolat = x.Destinolat,
                    Destinolog = x.Destinolog,
                    Tiempodemora = x.Tiempodemora,
                    Ruc = x.Ruc,
                    Fecharegistro = x.Fecharegistro,
                    Usuario = x.Usuario,
                    Base = x.Base,
                    Unidad = x.Unidad,
                    Ciconductor = x.Ciconductor,
                    Conductor = x.Conductor,
                    Unidadsiguiente = x.Unidadsiguiente,
                    Ciconductorsiguiente = x.Ciconductorsiguiente,
                    Conductorsiguiente = x.Conductorsiguiente,
                    Precio = x.Precio,
                    Km = x.Km,
                    Numvoucher = x.Numvoucher,
                    Valija = x.Valija,
                    Empleado = x.Empleado,
                    Recorrido = x.Recorrido,
                    Estado = x.Estado,
                    Autorizado = x.Autorizado,
                    CalleOrigen = BuscarCalle(x.Celular, x.Origenlat, x.Origenlog),
                    CalleDestino = BuscarCalle(x.Celular, x.Destinolat, x.Destinolog)
                })
                .ToList();
        }

        private static Pedido? BuscarPedido(DbAa5796GmoraContext context, PedidoIdentificadorDTO id)
        {
            return context.Pedidos.FirstOrDefault(p =>
                p.Celular == id.Celular &&
                p.Origenlat == id.Origenlat && p.Origenlog == id.Origenlog &&
                p.Destinolat == id.Destinolat && p.Destinolog == id.Destinolog &&
                p.Fecharegistro == id.FechaRegistroPedido);
        }

        // El conductor toma una carrera ya asignada a su unidad: pasa a PROCESO y arranca el
        // seguimiento GPS (taximetro en vivo se calcula en el cliente; aqui solo se guarda el
        // punto/hora de inicio).
        public void TomarCarrera(TomarCarreraRequestDTO request, string cedulaConductor)
        {
            using var context = new DbAa5796GmoraContext();

            var pedido = BuscarPedido(context, request);
            if (pedido == null)
                throw new InvalidOperationException("No se encontró la carrera.");

            pedido.Estado = "PROCESO";

            context.Carreraseguimientos.Add(new Carreraseguimiento
            {
                Celular = request.Celular,
                Origenlat = request.Origenlat,
                Origenlog = request.Origenlog,
                Destinolat = request.Destinolat,
                Destinolog = request.Destinolog,
                Fecharegistro = request.FechaRegistroPedido,
                Cedulaconductor = cedulaConductor,
                Fechainicio = DateTime.Now,
                Latinicio = request.LatInicio,
                Loginicio = request.LogInicio
            });

            context.SaveChanges();
        }

        // El conductor finaliza la carrera: pasa a FINALIZADO y escribe el precio/km finales
        // tambien en el propio Pedido (Facturacion/Voucher/reportes ya leen esos campos).
        public void FinalizarCarrera(FinalizarCarreraRequestDTO request)
        {
            using var context = new DbAa5796GmoraContext();

            var pedido = BuscarPedido(context, request);
            if (pedido == null)
                throw new InvalidOperationException("No se encontró la carrera.");

            pedido.Estado = "FINALIZADO";
            pedido.Precio = request.PrecioFinal;
            pedido.Km = request.DistanciaKm;

            var seguimiento = context.Carreraseguimientos
                .Where(s => s.Celular == request.Celular
                    && s.Origenlat == request.Origenlat && s.Origenlog == request.Origenlog
                    && s.Destinolat == request.Destinolat && s.Destinolog == request.Destinolog
                    && s.Fecharegistro == request.FechaRegistroPedido)
                .OrderByDescending(s => s.Idseguimiento)
                .FirstOrDefault();

            if (seguimiento != null)
            {
                seguimiento.Fechafin = DateTime.Now;
                seguimiento.Latfin = request.LatFin;
                seguimiento.Logfin = request.LogFin;
                seguimiento.Distanciakm = request.DistanciaKm;
                seguimiento.Preciofinal = request.PrecioFinal;
            }

            context.SaveChanges();
        }

        // "Mis Ganancias": carreras finalizadas por el conductor dentro de [desde, hasta]
        // (por fecha de finalizacion), con el total ganado y un historial corto.
        public GananciasConductorDTO GetGananciasConductor(string cedula, DateTime desde, DateTime hasta)
        {
            using var context = new DbAa5796GmoraContext();

            var hastaExclusivo = hasta.Date.AddDays(1);
            var carreras = context.Carreraseguimientos
                .Where(s => s.Cedulaconductor == cedula && s.Fechafin != null
                    && s.Fechafin >= desde.Date && s.Fechafin < hastaExclusivo)
                .OrderByDescending(s => s.Fechafin)
                .ToList();

            return new GananciasConductorDTO
            {
                CantidadCarreras = carreras.Count,
                TotalGanado = carreras.Sum(x => x.Preciofinal ?? 0),
                Historial = carreras.Select(x => new CarreraHistorialDTO
                {
                    Fecha = x.Fechafin!.Value,
                    Precio = x.Preciofinal,
                    DistanciaKm = x.Distanciakm
                }).ToList()
            };
        }

        // El despachador/admin califica el viaje desde la pantalla de Pedido existente; el
        // conductor solo la ve de solo lectura en su Resumen Final.
        public void CalificarCarrera(CalificarCarreraRequestDTO request)
        {
            using var context = new DbAa5796GmoraContext();

            var seguimiento = context.Carreraseguimientos
                .Where(s => s.Celular == request.Celular
                    && s.Origenlat == request.Origenlat && s.Origenlog == request.Origenlog
                    && s.Destinolat == request.Destinolat && s.Destinolog == request.Destinolog
                    && s.Fecharegistro == request.FechaRegistroPedido)
                .OrderByDescending(s => s.Idseguimiento)
                .FirstOrDefault();

            if (seguimiento == null)
                throw new InvalidOperationException("Esta carrera todavía no ha sido tomada por un conductor.");

            seguimiento.Calificacion = request.Calificacion;
            seguimiento.Comentariocalificacion = request.Comentario;

            context.SaveChanges();
        }

        public CalificarCarreraRequestDTO? GetCalificacionCarrera(PedidoIdentificadorDTO id)
        {
            using var context = new DbAa5796GmoraContext();

            var seguimiento = context.Carreraseguimientos
                .Where(s => s.Celular == id.Celular
                    && s.Origenlat == id.Origenlat && s.Origenlog == id.Origenlog
                    && s.Destinolat == id.Destinolat && s.Destinolog == id.Destinolog
                    && s.Fecharegistro == id.FechaRegistroPedido)
                .OrderByDescending(s => s.Idseguimiento)
                .FirstOrDefault();

            if (seguimiento == null || seguimiento.Calificacion == null) return null;

            return new CalificarCarreraRequestDTO
            {
                Celular = id.Celular,
                Origenlat = id.Origenlat,
                Origenlog = id.Origenlog,
                Destinolat = id.Destinolat,
                Destinolog = id.Destinolog,
                FechaRegistroPedido = id.FechaRegistroPedido,
                Calificacion = seguimiento.Calificacion.Value,
                Comentario = seguimiento.Comentariocalificacion
            };
        }
    }
}
