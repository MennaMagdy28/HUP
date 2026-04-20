using Microsoft.EntityFrameworkCore;
using HUP.BuildingBlocks.Infrastructure;
namespace HUP.Infrastructure
{
    public class AppDbContext : BaseDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Modules.Admin.Domain.Admin> Admins { get; set; }
        public DbSet<Modules.Students.Domain.Student> Students { get; set; }
        public DbSet<Modules.AcademicStructure.Domain.University> Universities { get; set; }
        public DbSet<Modules.AcademicStructure.Domain.Faculty> Faculties { get; set; }
        public DbSet<Modules.AcademicStructure.Domain.Department> Departments { get; set; }
        public DbSet<Modules.AcademicStructure.Domain.Levels> Levels { get; set; }
        public DbSet<Modules.AcademicStructure.Domain.CreditHours> CreditHours { get; set; }
        public DbSet<Modules.AcademicStructure.Domain.TermBasedTrack> TermBasedTracks { get; set; }
        public DbSet<Modules.Privileges.Domain.Role> Roles { get; set; }
        public DbSet<Modules.Privileges.Domain.Permission> Permissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
