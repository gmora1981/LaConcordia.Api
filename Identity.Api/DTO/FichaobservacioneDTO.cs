namespace Identity.Api.DTO
{
    public class FichaobservacioneDTO
    {
        public int Idfichaobs { get; set; }

        public string? Fkcedula { get; set; }

        public string? Fkunidad { get; set; }

        public DateOnly? Fecha { get; set; }

        public string? Motivo { get; set; }
    }
}
