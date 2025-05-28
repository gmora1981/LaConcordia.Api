using Identity.Api.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Identity.Api.Persistence.DataBase.configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Identity.Api.Persistence.DataBase
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)

        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Database schema
            builder.HasDefaultSchema("Identity");

            //Model constraints}
            ModelConfig(builder);
        }
        private void ModelConfig(ModelBuilder modelBuilder)
        {
            new ApplicationUserConfiguration(modelBuilder.Entity<ApplicationUser>());
            new ApplicationRoleConfiguration(modelBuilder.Entity<ApplicationRole>());
            new ApplicationUserRoleConfiguration(modelBuilder.Entity<ApplicationUserRole>());
        }
        // public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        //public DbSet<ApplicationUser> ApplicationUser { get; set; }
        //        public DbSet<IdentityUser> IdentityUser { get; set; }

    }
}
