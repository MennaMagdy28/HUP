using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HUP.Modules.AcademicStructure.Domain;
namespace HUP.Modules.AcademicStructure.Infrastructure.Configurations
{
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.HasMany(x => x.Faculties).WithOne(x => x.University).HasForeignKey(x => x.UniversityId);
        }
    }
}
