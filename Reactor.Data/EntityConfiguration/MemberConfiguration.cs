using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reactor.Core.Domain.Members;

namespace Reactor.Data.EntityConfiguration
{
    public class MemberConfiguration: IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
//            builder.ToTable("Members");
            
            builder.Property(m => m.FirstName).HasMaxLength(256).IsRequired();

            builder.Property(m => m.LastName).HasMaxLength(256).IsRequired();
        }
    }
}