using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Likes;

namespace Reactor.Data.EntityConfiguration
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.ToTable("Like");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.LikeById).HasMaxLength(450).IsRequired();

            builder.HasOne(l => l.Post)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.LikeBy)
                .WithMany(u => u.Likes)
                .HasForeignKey(l=>l.LikeById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}