using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Members;
using Reactor.Data.EntityConfiguration;

namespace Reactor.Data.EfContext
{
    public class ReactorDbContext : IdentityDbContext<IdentityUser>
    {
        public ReactorDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
         /*   builder.Entity<IdentityRole>(e => e.ToTable("Roles"));

            builder.Entity<IdentityUserRole<string>>(e => e.ToTable("MemberRoles"));

            builder.Entity<IdentityRoleClaim<string>>(e => e.ToTable("RoleClaims"));

            builder.Entity<IdentityUserClaim<string>>(e => e.ToTable("MemberClaims"));

            builder.Entity<IdentityUserLogin<string>>(e => e.ToTable("MemberLogins"));

            builder.Entity<IdentityUserToken<string>>(e => e.ToTable("MemberTokens"));
            */
            builder.ApplyConfiguration(new MemberConfiguration());

        }
    }
}