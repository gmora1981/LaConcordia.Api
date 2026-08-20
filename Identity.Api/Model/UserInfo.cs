namespace Identity.Api.Model
{
    public class UserInfo
    {
        public required string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }

        // Cedula del socio (Fichapersonal), solo aplica para usuarios con rol Taxista.
        public string? Cedula { get; set; }

        // Ruc de la empresa (Empresa), solo aplica para usuarios con rol Empresa.
        public string? Ruc { get; set; }
    }
}
