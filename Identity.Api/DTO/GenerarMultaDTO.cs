namespace Identity.Api.DTO
{
    public class GenerarMultaDTO
    {
        public string Idmulta { get; set; } = null!;
        public string Cidentidad { get; set; } = null!;
        public DateOnly Fecha { get; set; }
        public string? Observacion { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Abono { get; set; }
        public string? Tipo { get; set; }
    }
}
