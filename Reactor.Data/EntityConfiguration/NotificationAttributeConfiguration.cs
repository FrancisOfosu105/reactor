using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Notifications;

namespace Reactor.Data.EntityConfiguration
{
    public class NotificationAttributeConfiguration : IEntityTypeConfiguration<NotificationAttribute>
    {
        public void Configure(EntityTypeBuilder<NotificationAttribute> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Name).HasMaxLength(256).IsRequired();

            builder.Property(n => n.Value).HasMaxLength(256).IsRequired();

            builder.HasOne(n => n.Notification).WithMany(n => n.Attributes).HasForeignKey(n => n.NotificationId);
        }
    }
}