using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Messages;

namespace Reactor.Data.EntityConfiguration
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content).IsRequired();

            builder.Property(m => m.RecipientId).HasMaxLength(256).IsRequired();
            
            builder.Property(m => m.ChatId).HasMaxLength(256).IsRequired();

            
        }
    }
}