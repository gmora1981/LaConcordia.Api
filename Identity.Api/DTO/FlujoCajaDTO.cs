namespace Identity.Api.DTO
{
    public class FlujoCajaDTO
    {
        public int Idflujocaja { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Concepto { get; set; }
        public decimal? Ingreso { get; set; }
        public decimal? Egreso { get; set; }
        public decimal? Saldo { get; set; }
        public string? Observacion { get; set; }
        public string? Usuario { get; set; }
    }
}
