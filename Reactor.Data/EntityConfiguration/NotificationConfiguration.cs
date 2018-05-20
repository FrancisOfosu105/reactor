using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Notifications;

namespace Reactor.Data.EntityConfiguration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification> 
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.SenderId).HasMaxLength(256).IsRequired();

            builder.Property(n => n.RecipientId).HasMaxLength(450).IsRequired();

            builder.HasOne(n => n.Recipient)
                .WithMany(u => u.Notifications);
        }
    }
}