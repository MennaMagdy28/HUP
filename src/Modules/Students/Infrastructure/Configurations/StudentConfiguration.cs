using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace HUP.Modules.Students.Infrastructure.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Domain.Student>
    {
        public void Configure(EntityTypeBuilder<Domain.Student> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.UniversityCode).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.UniversityCode).IsUnique();
        }
    }
}
