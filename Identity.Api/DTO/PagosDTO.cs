namespace Identity.Api.DTO
{
    public class DeudaSocioDTO
    {
        public string Cedula { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }

        public List<GenerarCuotaDTO> CuotasPendientes { get; set; } = new();
        public List<GenerarMultaDTO> MultasPendientes { get; set; } = new();
        public List<GenerarPlanAyudaDTO> PlanAyudaPendientes { get; set; } = new();
        public List<GenerarPlanChoqueDTO> PlanChoquePendientes { get; set; } = new();

        public decimal TotalCuota => CuotasPendientes.Sum(x => (x.Valor ?? 0) - (x.Abono ?? 0));
        public decimal TotalMulta => MultasPendientes.Sum(x => (x.Valor ?? 0) - (x.Abono ?? 0));
        public decimal TotalPlanAyuda => PlanAyudaPendientes.Sum(x => (x.Valor ?? 0) - (x.Abono ?? 0));
        public decimal TotalPlanChoque => PlanChoquePendientes.Sum(x => (x.Valor ?? 0) - (x.Abono ?? 0));
        public decimal TotalAPagar => TotalCuota + TotalMulta + TotalPlanAyuda + TotalPlanChoque;
    }

    public class PagoCuotaRequestDTO
    {
        public string Periodo { get; set; } = null!;
        public string Semana { get; set; } = null!;
        public string Cidentidad { get; set; } = null!;
        public decimal ValorAPagar { get; set; }
        public string FormaDePago { get; set; } = null!;
        public string? Banco { get; set; }
        public string? NumComprobante { get; set; }
        public string? Detalle { get; set; }
    }

    // Tipo: "M" = Multa, "B" = Plan Ayuda (Beneficiario), "U" = Plan Choque (Unidad)
    public class PagoUbmRequestDTO
    {
        public string Ubm { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Cidentidad { get; set; } = null!;
        public decimal ValorAPagar { get; set; }
        public string FormaDePago { get; set; } = null!;
        public string? Banco { get; set; }
        public string? NumComprobante { get; set; }
        public string? Detalle { get; set; }
    }
}
