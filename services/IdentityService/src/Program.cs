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

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.MapInboundClaims = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "Authentication failed");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validated successfully for user {User}", context.Principal.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});

var serviceApiKey = builder.Configuration["ServiceApiKey"] ?? "";
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("InternalServiceOnly", policy =>
        policy
            .RequireAuthenticatedUser()
            .RequireAssertion(ctx =>
            {
                if (string.IsNullOrEmpty(serviceApiKey)) return false;
                if (ctx.Resource is not HttpContext httpContext) return false;
                return httpContext.Request.Headers["X-Service-Key"].ToString() == serviceApiKey;
            }));
});

// In-memory user store

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IResourcePermissionRepository, ResourcePermissionRepository>();
builder.Services.AddScoped<IResourcePermissionService, ResourcePermissionService>();
builder.Services.AddScoped<IClientTypeRepository, ClientTypeRepository>();
builder.Services.AddScoped<IGroupMemberRoleRepository, GroupMemberRoleRepository>();
builder.Services.AddScoped<IGroupMemberStatusRepository, GroupMemberStatusRepository>();
builder.Services.AddSingleton<ILookupCache, LookupCache>();
builder.Services.AddHostedService<LookupCacheWarmup>();

//Database
var conn = builder.Configuration.GetConnectionString("Identity");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(conn);
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<UserContext>(options => options.UseNpgsql(dataSource));

var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("Health");

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Identity Service API");
        options.AddHttpAuthentication("Bearer", _ => { });
         
    });
    
}

app.Run();
