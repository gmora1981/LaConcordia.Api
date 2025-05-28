using Identity.Api.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Api.Persistence.DataBase.configuration
{
    public class ApplicationUserConfiguration
    {
        public ApplicationUserConfiguration(EntityTypeBuilder<ApplicationUser> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            entityBuilder.Property(x => x.LastName).IsRequired().HasMaxLength(100);

            entityBuilder.HasMany(e => e.UserRoles).WithOne(e => e.User).HasForeignKey(e => e.UserId).IsRequired();
            // Agrega el usuario administrador
            var adminUser = new ApplicationUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9", // ID estático
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = "VVPCRDAS3MJWQD5CSW2GWPRADBXEZPN3" // Valor estático
            };

            // Genera un hash de contraseña - es importante hacerlo fuera del método HasData
            var hasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = "AQAAAAIAAYagAAAAEHTyWjdeP3XRzuoU2SXdE5meAAFDBhqzggt/5tyMJMn9CfwYu5/Jq+H1bF20tIgUYA=="; //hasher.HashPassword(adminUser, "Admin123!");

            entityBuilder.HasData(adminUser);
        }
    }
}
