using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class OrdenPagoRepository
    {
        private readonly DbAa5796GmoraContext _context;

        public OrdenPagoRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public List<PedidoVoucherDTO> GetPedidosPendientesVoucher(string? ruc = null)
        {
            using var context = new DbAa5796GmoraContext();

            var query = context.Pedidos
                .Where(p => p.Ruc != null && p.Ruc != "" && (p.Numvoucher == null || p.Numvoucher == ""));

            if (!string.IsNullOrEmpty(ruc))
                query = query.Where(p => p.Ruc == ruc);

            var pedidos = query.OrderByDescending(p => p.Fecharegistro).ToList();
            var empresas = context.Empresas.ToDictionary(e => e.Ruc, e => e.Razonsocial);

            return pedidos.Select(p => new PedidoVoucherDTO
            {
                Celular = p.Celular,
                Origenlat = p.Origenlat,
                Origenlog = p.Origenlog,
                Destinolat = p.Destinolat,
                Destinolog = p.Destinolog,
                Fecharegistro = p.Fecharegistro,
                Ruc = p.Ruc,
                RazonSocial = p.Ruc != null && empresas.ContainsKey(p.Ruc) ? empresas[p.Ruc] : null,
                Unidad = p.Unidad,
                Ciconductor = p.Ciconductor,
                Conductor = p.Conductor,
                Precio = p.Precio,
                Empleado = p.Empleado,
                Valija = p.Valija,
                Recorrido = p.Recorrido,
                Autorizado = p.Autorizado,
                Numvoucher = p.Numvoucher
            }).ToList();
        }

        public string? GetDireccionTexto(string celular, decimal lat, decimal lng)
        {
            using var context = new DbAa5796GmoraContext();

            var direccion = context.Direccions.FirstOrDefault(d =>
                d.Celular == celular && d.Latitud == lat && d.Longitud == lng);

            if (direccion == null) return null;

            return $"{direccion.Calle} {direccion.Numero} {direccion.Referencia}".Trim();
        }

        public decimal GetSaldoCajaActual()
        {
            using var context = new DbAa5796GmoraContext();
            var ultimo = context.Flujocajas.OrderByDescending(x => x.Idflujocaja).FirstOrDefault();
            return ultimo?.Saldo ?? 0m;
        }

        public OrdenPagoResultadoDTO GenerarOrdenPago(GenerarOrdenPagoRequestDTO request, string usuario)
        {
            var saldoAnterior = GetSaldoCajaActual();
            var saldoFinal = saldoAnterior - request.MontoAPagar;

            if (saldoFinal <= 0)
                throw new InvalidOperationException("Saldo negativo, no se puede ingresar la orden de pago.");

            // 1) Registro del voucher.
            _context.Ordendepagos.Add(new Ordendepago
            {
                Numvoucher = request.NumVoucher,
                Fechayhora = request.FechaYHora,
                Ruc = request.Ruc,
                Empresa = request.Empresa,
                Celular = request.Celular,
                Empleado = request.Empleado,
                Tiempoespera = request.TiempoEspera,
                Puntopartida = request.PuntoPartida,
                Recorrido = request.Recorrido,
                Puntofinal = request.PuntoFinal,
                Unidad = request.Unidad,
                Ciconductor = request.Ciconductor,
                Conductor = request.Conductor,
                Precio = request.Precio,
                Observacion = request.Observacion,
                Preciodesc = request.MontoAPagar,
                Fechaderegistro = DateTime.Now,
                Usuario = usuario,
                Valija = request.Valija,
                Cliente = request.Cliente
            });

            // 2) Marca el pedido original como facturado (voucher generado).
            var pedido = _context.Pedidos.FirstOrDefault(p =>
                p.Celular == request.Celular &&
                p.Origenlat == request.Origenlat && p.Origenlog == request.Origenlog &&
                p.Destinolat == request.Destinolat && p.Destinolog == request.Destinolog &&
                p.Fecharegistro == request.FechaRegistroPedido);

            if (pedido != null)
            {
                pedido.Numvoucher = request.NumVoucher;
                pedido.Precio = request.Precio;
                pedido.Recorrido = request.Recorrido;
                pedido.Empleado = request.Empleado;
                pedido.Usuario = usuario;
            }

            // 3) Auditoria del pedido facturado.
            _context.PedidoAuts.Add(new PedidoAut
            {
                Ruc = request.Ruc,
                Fecharegistro = request.FechaRegistroPedido,
                Usuario = usuario,
                Precio = request.Precio,
                Numvoucher = request.NumVoucher,
                Empleado = request.Empleado,
                Recorrido = request.Recorrido
            });

            // 4) Egreso de caja por el pago del voucher.
            _context.Flujocajas.Add(new Flujocaja
            {
                Fecha = DateTime.Now,
                Concepto = "EGRESO DE CAJA",
                Observacion = $"EN EFECTIVO POR PAGO DE VOUCHER # {request.NumVoucher}",
                Ingreso = 0,
                Egreso = request.MontoAPagar,
                Saldo = saldoFinal,
                Usuario = usuario
            });

            _context.SaveChanges();

            return new OrdenPagoResultadoDTO
            {
                SaldoAnterior = saldoAnterior,
                SaldoFinal = saldoFinal
            };
        }

        public List<OrdenPagoResumenDTO> GetOrdenPagoPorEmpresa(string ruc)
        {
            using var context = new DbAa5796GmoraContext();

            return context.Ordendepagos
                .Where(o => o.Ruc == ruc)
                .OrderByDescending(o => o.Fechayhora)
                .Select(o => new OrdenPagoResumenDTO
                {
                    Numvoucher = o.Numvoucher,
                    Fechayhora = o.Fechayhora,
                    Unidad = o.Unidad,
                    Conductor = o.Conductor,
                    Precio = o.Precio,
                    Preciodesc = o.Preciodesc,
                    Estadoproceso = o.Estadoproceso
                })
                .ToList();
        }
    }
}
