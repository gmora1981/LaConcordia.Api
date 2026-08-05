namespace Identity.Api.DTO
{
    public class BeneficiarioDTO
    {
        public string CiBeneficiario { get; set; } = null!;
        public int? Pkparentesco { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string CiAfiliado { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
    }

    public class GenerarPlanAyudaDTO
    {
        public string Beneficiario { get; set; } = null!;
        public string Cidentidad { get; set; } = null!;
        public DateOnly Fecha { get; set; }
        public string? Observacion { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Abono { get; set; }
    }

    public class GenerarPlanAyudaRequestDTO
    {
        public string Beneficiario { get; set; } = null!;
        public string CiAfiliado { get; set; } = null!;
        public string? Observacion { get; set; }
        public decimal Valor { get; set; }
    }

    public class GenerarPlanResultadoDTO
    {
        public int TotalGenerados { get; set; }
        public List<string> CedulasGeneradas { get; set; } = new();
    }
}
