using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Model
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole>? UserRoles { get; set; }
    }
}
