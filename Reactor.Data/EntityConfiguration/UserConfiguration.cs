using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Users;

namespace Reactor.Data.EntityConfiguration
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            
            builder.Property(m => m.FirstName).HasMaxLength(256).IsRequired();

            builder.Property(m => m.LastName).HasMaxLength(256).IsRequired();

            builder.Property(m => m.ProfilePictureUrl).HasMaxLength(256);
            
            builder.Property(m => m.ProfileCoverPictureUrl).HasMaxLength(256);
            
            builder.Property(m => m.Description).HasMaxLength(500);
            
            builder.Property(m => m.Lives).HasMaxLength(256);
            
            builder.Property(m => m.From).HasMaxLength(256);
            
            builder.Property(m => m.WorkAt).HasMaxLength(256);

        }
    }
}