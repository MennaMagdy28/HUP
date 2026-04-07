using HUP.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using System.Linq;
using System.Net.Http.Headers;

namespace HUP.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: true);
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<HupDbContext>));
                services.RemoveAll(typeof(DbContextOptions));

                services.AddDbContext<HupDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting_Integration"); // Make it distinct to avoid state overlap
                });

                services.RemoveAll(typeof(IConnectionMultiplexer));

                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<HupDbContext>();
                    db.Database.EnsureCreated(); // Ensure DB is clean
                    DatabaseSeeder.Initialize(db);
                }
            });

            builder.UseEnvironment("Testing");
            builder.UseSetting("detailedErrors", "true");
        }
    }
}
