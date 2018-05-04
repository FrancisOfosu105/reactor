using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Friends;

namespace Reactor.Data.EntityConfiguration
{
    public class FriendConfiguration : IEntityTypeConfiguration<Friend>
    {
        public void Configure(EntityTypeBuilder<Friend> builder)
        {
            builder.ToTable("Friend");

            builder.HasKey(f => new {f.RequestedById, f.RequestedToId});

            builder.Property(f => f.RequestedById).HasMaxLength(450).IsRequired();

            builder.Property(f => f.RequestedToId).HasMaxLength(450).IsRequired();

            builder
                .HasOne(f => f.RequestedBy)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(f => f.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(f => f.RequestedTo)
                .WithMany(m => m.ReceievedFriendRequests)
                .HasForeignKey(f => f.RequestedToId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}