using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Airport.Features.Auth.Infrastructure.Persistence;

public sealed class IdentityAuthDbContext(DbContextOptions<IdentityAuthDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>().ToTable("identity_users", "airport_app");
        modelBuilder.Entity<IdentityRole>().ToTable("identity_roles", "airport_app");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("identity_user_roles", "airport_app");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("identity_user_claims", "airport_app");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("identity_user_logins", "airport_app");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("identity_role_claims", "airport_app");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("identity_user_tokens", "airport_app");
    }
}
