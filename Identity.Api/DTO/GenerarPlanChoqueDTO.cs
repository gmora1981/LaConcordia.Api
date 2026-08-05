namespace Identity.Api.DTO
{
    public class GenerarPlanChoqueDTO
    {
        public string Unidad { get; set; } = null!;
        public string Cidentidad { get; set; } = null!;
        public DateOnly Fecha { get; set; }
        public string? Observacion { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Abono { get; set; }
    }

    public class GenerarPlanChoqueRequestDTO
    {
        public string Unidad { get; set; } = null!;
        public string? Observacion { get; set; }
        public decimal Valor { get; set; }
    }
}
