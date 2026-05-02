using IdentityService.Interfaces;
using IdentityService.Models;
using IdentityService.Repositories;
using IdentityService.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Scalar.AspNetCore;
using Shared.Models;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Basic services
builder.Services.AddHealthChecks();

// Controllers
builder.Services.AddControllers();

// OpenAPI metadata (used for contract generation tools like Scalar)
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings(
    Issuer: "IdentityService",
    Audience: "MealPlanner",
    Secret: "replace-this-with-a-secure-key-this-is-for-demo-use-only",
    ExpiresMinutes: 60);

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(new TokenService(jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.Secret));

// In-memory user store

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

//Database
var conn = builder.Configuration.GetConnectionString("Postgres");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(conn);
dataSourceBuilder.MapEnum<Shared.Models.ClientType>("client_type_enum");
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<UserContext>(options => options.UseNpgsql(dataSource));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("Health");

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();
