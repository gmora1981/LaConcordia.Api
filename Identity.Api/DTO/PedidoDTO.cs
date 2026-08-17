namespace Identity.Api.DTO
{
    public class GuardarDireccionRequestDTO
    {
        public string Celular { get; set; } = null!;
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public string? Calle { get; set; }
    }

    public class PedidoDTO
    {
        public string Celular { get; set; } = null!;
        public decimal Origenlat { get; set; }
        public decimal Origenlog { get; set; }
        public decimal Destinolat { get; set; }
        public decimal Destinolog { get; set; }
        public int? Tiempodemora { get; set; }
        public string? Ruc { get; set; }
        public DateTime Fecharegistro { get; set; }
        public string? Usuario { get; set; }
        public decimal? Base { get; set; }
        public string? Unidad { get; set; }
        public string? Ciconductor { get; set; }
        public string? Conductor { get; set; }
        public string? Unidadsiguiente { get; set; }
        public string? Ciconductorsiguiente { get; set; }
        public string? Conductorsiguiente { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Km { get; set; }
        public string? Numvoucher { get; set; }
        public string? Valija { get; set; }
        public string? Empleado { get; set; }
        public string? Recorrido { get; set; }
        public string? Estado { get; set; }
        public string? Autorizado { get; set; }
    }

    public class ConductorInfoDTO
    {
        public string Cedula { get; set; } = null!;
        public string? NombreCompleto { get; set; }
    }

    public class PrecioKmDTO
    {
        public decimal Precio { get; set; }
        public decimal Km { get; set; }
    }

    // Dashboard "Total de Ingresos de Carreras Asignadas": cantidad de pedidos registrados
    // por cada usuario (despachador) dentro de un rango de fechas.
    public class PedidosPorUsuarioDTO
    {
        public string Usuario { get; set; } = null!;
        public int Cantidad { get; set; }
    }

    // Dashboard "Top 10 de las Unidades con Mas Carreras" (FrmReporteUnidad -> "General" ->
    // RptUnidadEstadisticaPedido): cantidad de pedidos por unidad dentro de un rango de fechas.
    public class PedidosPorUnidadDTO
    {
        public string Unidad { get; set; } = null!;
        public int Cantidad { get; set; }
    }

    // "Reporte de Solicitud de Carrera" (FrmReporteOperadora, modo Detalle de Pedido /
    // RptOperadoraDetallePedidoUni): detalle de pedidos por usuario/operadora y rango de fechas.
    public class PedidoOperadoraDTO
    {
        public DateTime Fecharegistro { get; set; }
        public string? CalleOrigen { get; set; }
        public string? CalleDestino { get; set; }
        public string? Usuario { get; set; }
        public string? Unidad { get; set; }
        public decimal? Precio { get; set; }
    }
}
