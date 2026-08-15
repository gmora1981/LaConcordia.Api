namespace Identity.Api.DTO
{
    public class PedidoVoucherDTO
    {
        public string Celular { get; set; } = null!;
        public decimal Origenlat { get; set; }
        public decimal Origenlog { get; set; }
        public decimal Destinolat { get; set; }
        public decimal Destinolog { get; set; }
        public DateTime Fecharegistro { get; set; }
        public string? Ruc { get; set; }
        public string? RazonSocial { get; set; }
        public string? Unidad { get; set; }
        public string? Ciconductor { get; set; }
        public string? Conductor { get; set; }
        public decimal? Precio { get; set; }
        public string? Empleado { get; set; }
        public string? Valija { get; set; }
        public string? Recorrido { get; set; }
        public string? Autorizado { get; set; }
        public string? Numvoucher { get; set; }
    }

    public class GenerarOrdenPagoRequestDTO
    {
        public string Celular { get; set; } = null!;
        public decimal Origenlat { get; set; }
        public decimal Origenlog { get; set; }
        public decimal Destinolat { get; set; }
        public decimal Destinolog { get; set; }
        public DateTime FechaRegistroPedido { get; set; }

        public string NumVoucher { get; set; } = null!;
        public DateTime FechaYHora { get; set; }
        public string Ruc { get; set; } = null!;
        public string Empresa { get; set; } = null!;
        public string? Empleado { get; set; }
        public string? TiempoEspera { get; set; }
        public string? PuntoPartida { get; set; }
        public string? Recorrido { get; set; }
        public string? PuntoFinal { get; set; }
        public string Unidad { get; set; } = null!;
        public string? Ciconductor { get; set; }
        public string? Conductor { get; set; }
        public decimal Precio { get; set; }
        public string? Observacion { get; set; }
        public string? Valija { get; set; }
        public string? Cliente { get; set; }

        // Monto ya calculado por el frontend (precio - 10%, o precio +/- bono si "Sin Descuento").
        public decimal MontoAPagar { get; set; }
    }

    // "Modificar Datos": corrige Precio/Recorrido/Empleado del pedido SIN generar voucher,
    // sin tocar FlujoCaja ni la auditoria (igual que la opcion "Si" del dialogo del sistema de escritorio).
    public class ActualizarDatosPedidoRequestDTO
    {
        public string Celular { get; set; } = null!;
        public decimal Origenlat { get; set; }
        public decimal Origenlog { get; set; }
        public decimal Destinolat { get; set; }
        public decimal Destinolog { get; set; }
        public DateTime FechaRegistroPedido { get; set; }

        public decimal Precio { get; set; }
        public string? Recorrido { get; set; }
        public string? Empleado { get; set; }
    }

    public class OrdenPagoResultadoDTO
    {
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoFinal { get; set; }
    }

    public class OrdenPagoResumenDTO
    {
        public string Numvoucher { get; set; } = null!;
        public DateTime? Fechayhora { get; set; }
        public string? Unidad { get; set; }
        public string? Conductor { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Preciodesc { get; set; }
        public string? Estadoproceso { get; set; }

        // Usados en el PDF "Detalle de Factura" (RptEmpresaVoucher del escritorio).
        public string? Puntopartida { get; set; }
        public string? Recorrido { get; set; }
        public string? Puntofinal { get; set; }
        public string? Empleado { get; set; }
        public string? Observacion { get; set; }
    }
}
