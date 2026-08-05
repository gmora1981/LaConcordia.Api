namespace Identity.Api.DTO
{
    public class PasajeroDTO
    {
        public string Celular { get; set; } = null!;
        public string? Nombres { get; set; }
        public string? Observacion { get; set; }
        public string? Correo { get; set; }
        public string? Sexo { get; set; }
        public string? Whatsapp { get; set; }
        public int? Publicidad { get; set; }
    }
}
