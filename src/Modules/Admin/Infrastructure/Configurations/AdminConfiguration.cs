using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace HUP.Modules.Admin.Infrastructure.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Domain.Admin>
    {
        public void Configure(EntityTypeBuilder<Domain.Admin> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(255);
            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
