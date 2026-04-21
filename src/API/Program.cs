using HUP.API;
using HUP.Infrastructure;
using HUP.Tests;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Server=.;Database=HUP;Trusted_Connection=True;TrustServerCertificate=True");
}
);
builder.Services.AddScoped<LocalizationTestRunner>();
builder.Services.AddApplicationServices();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<LocalizationTestRunner>();
runner.Run();

// app.Run(); // Optional, if this is purely a test runner we might not need to start the web server
