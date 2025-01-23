using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Netflex.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User>(options)
{

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("dbo");
        builder.Entity<IdentityRole>().ToTable("tblRoles");
        builder.Entity<IdentityUserRole<string>>().ToTable("tblUserRoles");
        builder.Entity<IdentityUserToken<string>>().ToTable("tblUserTokens");
        builder.Entity<IdentityUserLogin<string>>().ToTable("tblUserLogins");
        builder.Entity<IdentityUserClaim<string>>().ToTable("tblUserClaims");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("tblRoleClaims");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}