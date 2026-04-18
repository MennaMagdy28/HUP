using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Application;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                // Configure your database provider here, e.g., Npgsql, SqlServer, etc.
                // options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Register IUnitOfWork
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());

            // Add other global infrastructure services

            return services;
        }
    }
}