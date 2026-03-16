using Shared.Models;
using Shared.Services;
using AuthService.Services;
using Scalar.AspNetCore;

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
    Issuer: "AuthService",
    Audience: "MealPlanner",
    Secret: "replace-this-with-a-secure-key",
    ExpiresMinutes: 60);

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(new JwtService(jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.Secret));

// In-memory user store
builder.Services.AddSingleton<UserStore>();

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
