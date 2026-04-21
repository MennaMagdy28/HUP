using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HUP.Modules.AcademicStructure.Domain;
namespace HUP.Modules.AcademicStructure.Infrastructure.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.IsProgram).IsRequired();
            builder.HasMany(x => x.Levels).WithOne(x => x.Department).HasForeignKey(x => x.DepartmentId);
        }
    }
}
