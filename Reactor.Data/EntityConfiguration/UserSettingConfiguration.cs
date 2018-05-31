using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Users;

namespace Reactor.Data.EntityConfiguration
{
    public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
    {
        public void Configure(EntityTypeBuilder<UserSetting> builder)
        {
            builder.ToTable("UserSetting");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.UserId).HasMaxLength(450).IsRequired();

            builder.HasOne(u => u.User).WithOne(u => u.UserSetting)
                .HasForeignKey<UserSetting>(u => u.UserId);
        }
    }
}