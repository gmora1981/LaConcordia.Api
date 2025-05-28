using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Model
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public ICollection<ApplicationUserRole>? UserRoles { get; set; }
    }
}
