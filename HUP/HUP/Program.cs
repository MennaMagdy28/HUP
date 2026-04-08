using HUP.Application.Services.Caching;
using HUP.Application.Services.Implementations;
using HUP.Application.Services.Interfaces;
using HUP.Common.Extensions;
using HUP.Core.Entities.Identity;
using HUP.Core.Interfaces;
using HUP.Data;
using HUP.Repositories.Implementations;
using HUP.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;
using FluentValidation;
using HUP.Application.Validators;
using HUP.API.Filters;
using HUP.API.Middleware;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

if (builder.Environment.IsEnvironment("Testing"))
{
    // Do not register DbContext here, let WebApplicationFactory do it
}
else if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<HupDbContext>(options =>
        options.UseSqlServer(connectionString)
    );
}
else
{
    builder.Services.AddDbContext<HupDbContext>(options =>
        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=HUP_DesignDb;Trusted_Connection=True;MultipleActiveResultSets=true")
    );
}

// redis connection
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
}
//cache service (singleton)
builder.Services.AddSingleton<ICacheService, CacheService>();

// This registers Hasher, UserManager, SignInManager, etc.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>(); 
//tracks all services and repositories (DI)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddApplicationServices();

builder.Services.AddScoped<IAuthorizationHandler, HUP.API.Permissions.PermissionAuthorizationHandler>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in HUP.Core.Constants.AppPermissions.GetAll())
    {
        options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new HUP.API.Permissions.PermissionRequirement(permission)));
    }
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "HUP API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HupDbContext>();
    // Apply pending migrations if any
    if (context.Database.IsRelational())
    {
        context.Database.Migrate();
    }
    // Seed the comprehensive test data
    HUP.Data.DatabaseSeeder.Initialize(context);
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseCors("AllowReactApp");
app.UseStaticFiles();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
