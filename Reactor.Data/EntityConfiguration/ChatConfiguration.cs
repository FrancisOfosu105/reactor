using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Chats;

namespace Reactor.Data.EntityConfiguration
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable("Chat");

            builder.HasKey(c => c.ChatId);

            builder.Property(c => c.ChatId).HasMaxLength(256).IsRequired();

            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .HasForeignKey(m => m.ChatId);
        }
    }
}