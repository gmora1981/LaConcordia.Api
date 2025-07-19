using Identity.Api.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Api.Persistence.DataBase.configuration
{
    public class ApplicationUserRoleConfiguration
    {
        public ApplicationUserRoleConfiguration(EntityTypeBuilder<ApplicationUserRole> entityBuilder)
        {
           // entityBuilder.HasKey(x => new { x.UserId, x.RoleId });

            // Asigna el rol Admin al usuario admin
            entityBuilder.HasData(
                new ApplicationUserRole
                {
                    RoleId = "ee0e3def-49f9-4dbf-8889-2c844e66f74a",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    //Discriminator = "ApplicationUserRole"
                }
            );
        }
    }
}
