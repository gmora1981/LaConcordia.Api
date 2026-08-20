using Identity.Api.DTO;
using Identity.Api.Reporteria;
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
            try
            {
                using var context = new DbAa5796GmoraContext();

                var direccion = context.Direccions.FirstOrDefault(d =>
                    d.Celular == celular && d.Latitud == lat && d.Longitud == lng);

                if (direccion == null) return null;

                return $"{direccion.Calle} {direccion.Numero} {direccion.Referencia}".Trim();
            }
            catch
            {
                // Coordenadas corruptas o fuera del rango de la columna (dato historico invalido):
                // se omite la direccion en vez de tumbar todo el modal de Orden de Pago.
                return null;
            }
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

        // "Modificar Datos": solo corrige Precio/Recorrido/Empleado del pedido, sin generar
        // voucher ni tocar FlujoCaja/auditoria.
        public void ActualizarDatosPedido(ActualizarDatosPedidoRequestDTO request, string usuario)
        {
            var pedido = _context.Pedidos.FirstOrDefault(p =>
                p.Celular == request.Celular &&
                p.Origenlat == request.Origenlat && p.Origenlog == request.Origenlog &&
                p.Destinolat == request.Destinolat && p.Destinolog == request.Destinolog &&
                p.Fecharegistro == request.FechaRegistroPedido);

            if (pedido == null)
                throw new Exception("No se encontró el pedido a modificar.");

            pedido.Precio = request.Precio;
            pedido.Recorrido = request.Recorrido;
            pedido.Empleado = request.Empleado;
            pedido.Usuario = usuario;

            _context.SaveChanges();
        }

        public List<OrdenPagoResumenDTO> GetOrdenPagoPorEmpresa(string ruc, DateTime? hasta = null, string? estadoProceso = "a")
        {
            using var context = new DbAa5796GmoraContext();

            var query = context.Ordendepagos.Where(o => o.Ruc == ruc);

            // Por defecto solo se muestran los vouchers pendientes de facturar (estadoproceso
            // "a"); los ya procesados ("i") no deberian seguir apareciendo aqui. Los registros
            // sin estado (nulos, de datos historicos) se tratan como pendientes.
            if (!string.IsNullOrEmpty(estadoProceso))
            {
                query = query.Where(o => o.Estadoproceso == estadoProceso || o.Estadoproceso == null);
            }

            if (hasta.HasValue)
            {
                // "Hasta" incluye todo el dia seleccionado, igual que el resto de reportes.
                var hastaExclusivo = hasta.Value.Date.AddDays(1);
                query = query.Where(o => o.Fechayhora < hastaExclusivo);
            }

            return query
                .OrderByDescending(o => o.Fechayhora)
                .Select(o => new OrdenPagoResumenDTO
                {
                    Numvoucher = o.Numvoucher,
                    Fechayhora = o.Fechayhora,
                    Unidad = o.Unidad,
                    Conductor = o.Conductor,
                    Precio = o.Precio,
                    Preciodesc = o.Preciodesc,
                    Estadoproceso = o.Estadoproceso,
                    Puntopartida = o.Puntopartida,
                    Recorrido = o.Recorrido,
                    Puntofinal = o.Puntofinal,
                    Empleado = o.Empleado,
                    Observacion = o.Observacion
                })
                .ToList();
        }

        public byte[] ExportarFacturacionPdf(string ruc, string razonSocial, DateTime? hasta, string usuario)
        {
            var lista = GetOrdenPagoPorEmpresa(ruc, hasta);
            return FacturacionPdfGenerator.GenerarPdf(lista, ruc, razonSocial, hasta, usuario);
        }

        // "Reporte de Voucher por Pagar": misma regla de "pendiente de voucher" que usa Orden
        // de Pago (Ruc asignado, Numvoucher aun vacio), acotada a [desde, hasta] y filtrada
        // opcionalmente por unidad.
        public List<ReporteVoucherPagarDTO> GetVouchersPendientesPorUnidad(string? unidad, DateTime desde, DateTime hasta)
        {
            using var context = new DbAa5796GmoraContext();

            var hastaExclusivo = hasta.Date.AddDays(1);
            var query = context.Pedidos
                .Where(p => p.Ruc != null && p.Ruc != "" && (p.Numvoucher == null || p.Numvoucher == "")
                    && p.Fecharegistro >= desde.Date && p.Fecharegistro < hastaExclusivo);

            if (!string.IsNullOrEmpty(unidad))
                query = query.Where(p => p.Unidad == unidad);

            var pedidos = query.OrderBy(p => p.Fecharegistro).ToList();

            var empresas = context.Empresas.ToDictionary(e => e.Ruc, e => e.Razonsocial);

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

            return pedidos.Select(p => new ReporteVoucherPagarDTO
            {
                Fecharegistro = p.Fecharegistro,
                CalleOrigen = BuscarCalle(p.Celular, p.Origenlat, p.Origenlog),
                CalleDestino = BuscarCalle(p.Celular, p.Destinolat, p.Destinolog),
                Empresa = p.Ruc != null && empresas.ContainsKey(p.Ruc) ? empresas[p.Ruc] : null,
                Precio = p.Precio
            }).ToList();
        }

        // Dashboard "Vouchers Emitidos": total de vouchers generados en el rango (por
        // Fechayhora), separados en procesados/pagados (Estadoproceso = "i") y pendientes
        // ("a" o nulo, igual criterio que Facturacion).
        public ResumenVoucherDTO GetResumenVouchers(DateTime desde, DateTime hasta, string? ruc = null)
        {
            using var context = new DbAa5796GmoraContext();

            var hastaExclusivo = hasta.Date.AddDays(1);
            var query = context.Ordendepagos
                .Where(o => o.Fechayhora >= desde.Date && o.Fechayhora < hastaExclusivo);

            if (!string.IsNullOrEmpty(ruc))
                query = query.Where(o => o.Ruc == ruc);

            var vouchers = query.ToList();

            var procesados = vouchers.Where(o => o.Estadoproceso == "i").ToList();
            var pendientes = vouchers.Where(o => o.Estadoproceso != "i").ToList();

            return new ResumenVoucherDTO
            {
                Total = vouchers.Count,
                Procesados = procesados.Count,
                Pendientes = pendientes.Count,
                MontoTotal = vouchers.Sum(x => x.Precio ?? 0),
                MontoProcesado = procesados.Sum(x => x.Precio ?? 0),
                MontoPendiente = pendientes.Sum(x => x.Precio ?? 0)
            };
        }

        public byte[] ExportarReporteVoucherPagarPdf(string? unidad, DateTime desde, DateTime hasta, string usuarioLogueado)
        {
            var lista = GetVouchersPendientesPorUnidad(unidad, desde, hasta);
            var conductor = !string.IsNullOrEmpty(unidad)
                ? new PedidoRepository().GetConductorPorUnidad(unidad)?.NombreCompleto
                : null;
            return ReporteVoucherPagarPdfGenerator.GenerarPdf(lista, unidad, conductor, desde, hasta, usuarioLogueado);
        }
    }
}
