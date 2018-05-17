using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Follows;

namespace Reactor.Data.EntityConfiguration
{
    public class FollowConfiguration : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.ToTable("Follow");

            builder.HasKey(f => new {f.FollowerId, f.FolloweeId});

            builder.Property(f => f.FolloweeId).HasMaxLength(450).IsRequired();
            
            builder.Property(f => f.FollowerId).HasMaxLength(450).IsRequired();

            builder.HasOne(f => f.Follower)
                .WithMany(f => f.Followees)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Followee)
                .WithMany(f => f.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}