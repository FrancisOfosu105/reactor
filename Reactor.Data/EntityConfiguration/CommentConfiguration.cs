using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Comments;

namespace Reactor.Data.EntityConfiguration
{
    public class CommentConfiguration: IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comment");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CommentById).HasMaxLength(450).IsRequired();

            builder.Property(c => c.Content).IsRequired();

            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);

            builder.HasOne(c => c.CommentBy)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.CommentById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}