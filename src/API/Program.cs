using HUP.Infrastructure;
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
builder.Services.AddInfrastructureServices();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseMiddleware<HUP.API.Middleware.ExceptionMiddleware>();
app.UseMiddleware<HUP.API.Middleware.AuthMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
