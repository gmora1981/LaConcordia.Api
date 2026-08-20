namespace Identity.Api.DTO
{
    // Solicitud de carrera hecha por una empresa desde el portal, pendiente de que el
    // despachador la convierta en un Pedido (mismo flujo que hoy hace manualmente con el
    // checkbox "Facturar a Empresa" en Gestion de Pedidos).
    public class SolicitudCarreraDTO
    {
        public int Idsolicitud { get; set; }
        public string Ruc { get; set; } = null!;
        public string? RazonSocial { get; set; }
        public string Celular { get; set; } = null!;
        public string? Empleado { get; set; }
        public decimal Origenlat { get; set; }
        public decimal Origenlog { get; set; }
        public decimal? Destinolat { get; set; }
        public decimal? Destinolog { get; set; }
        public string? Observacion { get; set; }
        public DateTime Fechasolicitud { get; set; }
        public string Estado { get; set; } = null!;
    }

    public class CrearSolicitudCarreraRequestDTO
    {
        public string Celular { get; set; } = null!;
        public string? Empleado { get; set; }
        public decimal Origenlat { get; set; }
        public decimal Origenlog { get; set; }
        public decimal? Destinolat { get; set; }
        public decimal? Destinolog { get; set; }
        public string? Observacion { get; set; }
    }
}
