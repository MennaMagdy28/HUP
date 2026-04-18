using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDbContext : BaseDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations for all modules here if needed or scan assemblies
        }
    }
}