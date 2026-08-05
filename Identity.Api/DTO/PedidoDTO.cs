namespace Identity.Api.DTO
{
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
}
