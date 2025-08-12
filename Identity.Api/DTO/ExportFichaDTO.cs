namespace Identity.Api.DTO
{
    public class ExportFichaDTO
    {
        public FichapersonalDTO Ficha { get; set; }
        public UnidadDTO Unidad { get; set; }
        public DuenopuestoDTO Duenopuesto { get; set; }
        public List<SegurovidumDTO> Beneficiarios { get; set; }
    }
}
