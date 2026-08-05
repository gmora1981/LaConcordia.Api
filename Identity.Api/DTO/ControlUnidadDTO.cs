namespace Identity.Api.DTO
{
    public class UnidadServicioDTO
    {
        public string Fkunidad { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
    }

    // Direccion: "INGRESO" (entra a servicio) o "SALIDA" (sale de servicio).
    public class MoverUnidadRequestDTO
    {
        public string Unidad { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string Turno { get; set; } = null!;
        public string Direccion { get; set; } = null!;
    }
}
