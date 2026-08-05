namespace Identity.Api.DTO
{
    public class GenerarCuotaDTO
    {
        public string Periodo { get; set; } = null!;
        public string Semana { get; set; } = null!;
        public string Cidentidad { get; set; } = null!;
        public decimal? Valor { get; set; }
        public DateOnly? Fecha { get; set; }
        public decimal? Abono { get; set; }
    }

    public class PendienteCuotaDTO
    {
        public string Cedula { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public decimal? Cuotaf { get; set; }
    }

    public class GenerarCuotaResultadoDTO
    {
        public int TotalGenerados { get; set; }
        public List<string> CedulasGeneradas { get; set; } = new();
    }
}
