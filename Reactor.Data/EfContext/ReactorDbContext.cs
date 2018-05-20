using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Users;
using Reactor.Data.EntityConfiguration;

namespace Reactor.Data.EfContext
{
    public class ReactorDbContext : IdentityDbContext<User>
    {
        public ReactorDbContext(DbContextOptions options)
            : base(options){}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Role"); });

            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });

            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });

            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });

            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });

            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });

            builder.ApplyConfiguration(new UserConfiguration());

            builder.ApplyConfiguration(new FriendConfiguration());

            builder.ApplyConfiguration(new PostConfiguration());
            
            builder.ApplyConfiguration(new PhotoConfiguration());
            
            builder.ApplyConfiguration(new CommentConfiguration());

            builder.ApplyConfiguration(new LikeConfiguration());
            
            builder.ApplyConfiguration(new FollowConfiguration());
            
            builder.ApplyConfiguration(new MessageConfiguration());
            
            builder.ApplyConfiguration(new ChatConfiguration());

            builder.ApplyConfiguration(new NotificationConfiguration());

            builder.ApplyConfiguration(new NotificationAttributeConfiguration());

        }
    }
}