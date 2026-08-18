using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Model
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        // Vincula la cuenta de un conductor (rol "Taxista") a su Fichapersonal.Cedula, en la
        // base de datos de negocio. Nulo para usuarios administrativos/despachadores.
        public string? Cedula { get; set; }

        public ICollection<ApplicationUserRole>? UserRoles { get; set; }
    }
}
