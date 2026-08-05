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
    }
}
