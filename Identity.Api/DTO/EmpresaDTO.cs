namespace Identity.Api.DTO
{
    public class EmpresaDTO
    {
        public string Ruc { get; set; } = null!;

        public string? Razonsocial { get; set; }

        public string? Direccion { get; set; }

        public string? Telefono { get; set; }

        public string? Estado { get; set; }
    }
}
