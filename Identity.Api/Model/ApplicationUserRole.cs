using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Model
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationRole Role { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

        // Add the Discriminator property to resolve the error
        public string Discriminator { get; set; } = null!;
    }
}
