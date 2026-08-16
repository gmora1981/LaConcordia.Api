using Identity.Api.DTO;
using Identity.Api.Paginado;
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
    }
}
