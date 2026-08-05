using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class PagosRepository
    {
        private static readonly string[] FormasSinComprobante = { "EFECTIVO", "EXONERACION", "INACTIVO" };

        private readonly DbAa5796GmoraContext _context;

        public PagosRepository()
        {
            _context = new DbAa5796GmoraContext();
        }

        public DeudaSocioDTO? GetDeudaSocio(string cedula)
        {
            using var context = new DbAa5796GmoraContext();

            var socio = context.Fichapersonals.FirstOrDefault(f => f.Cedula == cedula);
            if (socio == null) return null;

            var deuda = new DeudaSocioDTO
            {
                Cedula = socio.Cedula,
                Nombre = socio.Nombre,
                Apellidos = socio.Apellidos
            };

            deuda.CuotasPendientes = context.Generarcuota
                .Where(c => c.Cidentidad == cedula && (c.Valor ?? 0) > (c.Abono ?? 0))
                .Select(c => new GenerarCuotaDTO
                {
                    Periodo = c.Periodo,
                    Semana = c.Semana,
                    Cidentidad = c.Cidentidad,
                    Valor = c.Valor,
                    Fecha = c.Fecha,
                    Abono = c.Abono
                })
                .ToList();

            deuda.MultasPendientes = context.Generarmulta
                .Where(m => m.Cidentidad == cedula && (m.Valor ?? 0) > (m.Abono ?? 0))
                .Select(m => new GenerarMultaDTO
                {
                    Idmulta = m.Idmulta,
                    Cidentidad = m.Cidentidad,
                    Fecha = m.Fecha,
                    Observacion = m.Observacion,
                    Valor = m.Valor,
                    Abono = m.Abono,
                    Tipo = m.Tipo
                })
                .ToList();

            deuda.PlanAyudaPendientes = context.Generarplanayuda
                .Where(p => p.Cidentidad == cedula && (p.Valor ?? 0) > (p.Abono ?? 0))
                .Select(p => new GenerarPlanAyudaDTO
                {
                    Beneficiario = p.Beneficiario,
                    Cidentidad = p.Cidentidad,
                    Fecha = p.Fecha,
                    Observacion = p.Observacion,
                    Valor = p.Valor,
                    Abono = p.Abono
                })
                .ToList();

            deuda.PlanChoquePendientes = context.Generarplanchoques
                .Where(p => p.Cidentidad == cedula && (p.Valor ?? 0) > (p.Abono ?? 0))
                .Select(p => new GenerarPlanChoqueDTO
                {
                    Unidad = p.Unidad,
                    Cidentidad = p.Cidentidad,
                    Fecha = p.Fecha,
                    Observacion = p.Observacion,
                    Valor = p.Valor,
                    Abono = p.Abono
                })
                .ToList();

            return deuda;
        }

        public bool ExisteComprobante(string banco, string numComprobante)
        {
            using var context = new DbAa5796GmoraContext();

            var enCuota = context.Movimientocuota.Any(m => m.Banco == banco && m.Numcomprobante == numComprobante);
            var enUbm = context.Movimientoubms.Any(m => m.Banco == banco && m.Numcomprobante == numComprobante);
            return enCuota || enUbm;
        }

        private void ValidarComprobanteSiAplica(string formaDePago, string? banco, string? numComprobante)
        {
            if (FormasSinComprobante.Contains(formaDePago.ToUpper()))
                return;

            if (string.IsNullOrWhiteSpace(numComprobante))
                throw new InvalidOperationException("Debe ingresar el número de comprobante para esta forma de pago.");

            if (ExisteComprobante(banco ?? formaDePago, numComprobante))
                throw new InvalidOperationException("El comprobante ya existe. Verifique el número ingresado.");
        }

        public void PagarCuota(PagoCuotaRequestDTO request, string usuario)
        {
            if (request.ValorAPagar <= 0)
                throw new InvalidOperationException("El valor a pagar debe ser mayor a cero.");

            var cuota = _context.Generarcuota.FirstOrDefault(c =>
                c.Periodo == request.Periodo && c.Semana == request.Semana && c.Cidentidad == request.Cidentidad);

            if (cuota == null)
                throw new InvalidOperationException("No se encontró la cuota a pagar.");

            var saldo = (cuota.Valor ?? 0) - (cuota.Abono ?? 0);
            if (request.ValorAPagar > saldo)
                throw new InvalidOperationException("El valor a cancelar es mayor al saldo. Favor corregir.");

            ValidarComprobanteSiAplica(request.FormaDePago, request.Banco, request.NumComprobante);

            _context.Movimientocuota.Add(new Movimientocuotum
            {
                Periodo = request.Periodo,
                Semana = request.Semana,
                Cidentidad = request.Cidentidad,
                Fechapago = DateTime.Now,
                Valorpagado = request.ValorAPagar,
                Formadepago = request.FormaDePago,
                Detalle = request.Detalle,
                Usuario = usuario,
                Banco = request.Banco ?? request.FormaDePago,
                Numcomprobante = request.NumComprobante
            });

            cuota.Abono = (cuota.Abono ?? 0) + request.ValorAPagar;

            _context.SaveChanges();
        }

        public void PagarUbm(PagoUbmRequestDTO request, string usuario)
        {
            if (request.ValorAPagar <= 0)
                throw new InvalidOperationException("El valor a pagar debe ser mayor a cero.");

            decimal valor, abono;

            switch (request.Tipo)
            {
                case "M":
                    var multa = _context.Generarmulta.FirstOrDefault(m =>
                        m.Idmulta == request.Ubm && m.Cidentidad == request.Cidentidad);
                    if (multa == null) throw new InvalidOperationException("No se encontró la multa a pagar.");
                    valor = multa.Valor ?? 0; abono = multa.Abono ?? 0;
                    ValidarSaldo(request.ValorAPagar, valor, abono);
                    ValidarComprobanteSiAplica(request.FormaDePago, request.Banco, request.NumComprobante);
                    RegistrarMovimientoUbm(request, usuario);
                    multa.Abono = abono + request.ValorAPagar;
                    break;

                case "B":
                    var ayuda = _context.Generarplanayuda.FirstOrDefault(p =>
                        p.Beneficiario == request.Ubm && p.Cidentidad == request.Cidentidad);
                    if (ayuda == null) throw new InvalidOperationException("No se encontró el plan de ayuda a pagar.");
                    valor = ayuda.Valor ?? 0; abono = ayuda.Abono ?? 0;
                    ValidarSaldo(request.ValorAPagar, valor, abono);
                    ValidarComprobanteSiAplica(request.FormaDePago, request.Banco, request.NumComprobante);
                    RegistrarMovimientoUbm(request, usuario);
                    ayuda.Abono = abono + request.ValorAPagar;
                    break;

                case "U":
                    var choque = _context.Generarplanchoques.FirstOrDefault(p =>
                        p.Unidad == request.Ubm && p.Cidentidad == request.Cidentidad);
                    if (choque == null) throw new InvalidOperationException("No se encontró el plan de choque a pagar.");
                    valor = choque.Valor ?? 0; abono = choque.Abono ?? 0;
                    ValidarSaldo(request.ValorAPagar, valor, abono);
                    ValidarComprobanteSiAplica(request.FormaDePago, request.Banco, request.NumComprobante);
                    RegistrarMovimientoUbm(request, usuario);
                    choque.Abono = abono + request.ValorAPagar;
                    break;

                default:
                    throw new InvalidOperationException("Tipo de pago no reconocido.");
            }

            _context.SaveChanges();
        }

        private static void ValidarSaldo(decimal valorAPagar, decimal valor, decimal abono)
        {
            var saldo = valor - abono;
            if (valorAPagar > saldo)
                throw new InvalidOperationException("El valor a cancelar es mayor al saldo. Favor corregir.");
        }

        private void RegistrarMovimientoUbm(PagoUbmRequestDTO request, string usuario)
        {
            _context.Movimientoubms.Add(new Movimientoubm
            {
                Ubm = request.Ubm,
                Tipo = request.Tipo,
                Cidentidad = request.Cidentidad,
                Fechapago = DateTime.Now,
                Valorpagado = request.ValorAPagar,
                Formadepago = request.FormaDePago,
                Detalle = request.Detalle,
                Usuario = usuario,
                Banco = request.Banco ?? request.FormaDePago,
                Numcomprobante = request.NumComprobante
            });
        }
    }
}
