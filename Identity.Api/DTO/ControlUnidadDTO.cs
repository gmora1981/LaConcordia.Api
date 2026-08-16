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

    // Un registro del historial de ingresos/salidas (tabla Controlunidades).
    public class ControlUnidadMovimientoDTO
    {
        public DateTime Fecharegistro { get; set; }
        public string? Turno { get; set; }
        public string Unidad { get; set; } = null!;
        public string? Ciconductor { get; set; }
        public string? Conductor { get; set; }
        public string Estado { get; set; } = null!;
        public string? Monitora { get; set; }
    }
}
