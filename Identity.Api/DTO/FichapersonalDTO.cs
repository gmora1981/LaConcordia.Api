namespace Identity.Api.DTO
{
    public class FichapersonalDTO
    {
        public string Cedula { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
        public string? Correo { get; set; }
        public DateOnly? Fechanacimiento { get; set; }
        public DateTime? Fechaingreso { get; set; }
        public string? Domicilio { get; set; }
        public string? Referencia { get; set; }
        public string? Estado { get; set; }
        public string? Estadoservicio { get; set; }
        public decimal? Cuotaf { get; set; }

        // Relaciones simplificadas para mostrar descripción o ID
        public int? Fknacionalidad { get; set; }
        public string? NacionalidadDescripcion { get; set; }

        public int? Fkestadocivil { get; set; }
        public string? EstadoCivilDescripcion { get; set; }

        public int? Fktipolicencia { get; set; }
        public string? TipoLicenciaDescripcion { get; set; }

        public int? Fkcargo { get; set; }
        public string? CargoDescripcion { get; set; }

        public int? Fkniveleducacion { get; set; }
        public string? NivelEducacionDescripcion { get; set; }

        public string? Fkunidad { get; set; }
        public string? UnidadDescripcion { get; set; }  // si deseas mostrar "Unidad1", "Placa", etc.

        public string? Fkdpuesto { get; set; }
        public string? DuenoCedula { get; set; }
        public string? DuenoNombres { get; set; }
        public string? DuenoApellidos { get; set; }
    }
}
