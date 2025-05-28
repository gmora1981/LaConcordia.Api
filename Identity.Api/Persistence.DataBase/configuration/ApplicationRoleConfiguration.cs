using Identity.Api.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Api.Persistence.DataBase.configuration
{
    public class ApplicationRoleConfiguration
    {
        public ApplicationRoleConfiguration(EntityTypeBuilder<ApplicationRole> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.HasData(

                new ApplicationRole
                {
                   // Usar un ID estático en lugar de Guid.NewGuid()
                    Id = "ee0e3def-49f9-4dbf-8889-2c844e66f74a",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }

                );

            entityBuilder.HasMany(e => e.UserRoles).WithOne(e => e.Role).HasForeignKey(e => e.RoleId).IsRequired();

        }
    }
}
