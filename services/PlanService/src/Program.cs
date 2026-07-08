using Shared.Models;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PlanService.Repositories;
using PlanService.Interfaces;
using PlanService.Services;
 

var builder = WebApplication.CreateBuilder(args);

// Basic services
builder.Services.AddHealthChecks();

// Controllers
builder.Services.AddControllers();

// OpenAPI metadata (used for contract generation tools like Scalar)
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings(
    Issuer: "IdentityService",
    Audience: "MealPlanner",
    Secret: "replace-this-with-a-secure-key-this-is-for-demo-use-only",
    ExpiresMinutes: 60);

builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
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
});
//repositories
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IResourcePermissionRepository, ResourcePermissionRepository>();
builder.Services.AddScoped<ICachedUserRepository, CachedUserRepository>();
builder.Services.AddScoped<ICachedGroupRepository, CachedGroupRepository>();
builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();
builder.Services.AddScoped<ISubjectTypeRepository, SubjectTypeRepository>();
builder.Services.AddScoped<IPermissionTypeRepository,PermissionTypeRepository>();
builder.Services.AddScoped<IPlanMealItemStatusTypeRepository, MealItemPlanStatusTypeRepository>();
builder.Services.AddScoped<IMealPlanRepository, MealPlanRepository>();
builder.Services.AddScoped<IMealItemPlanRepository, MealItemPlanRepository>();
//services
builder.Services.AddScoped<IPlanningService, PlanningService>();
builder.Services.AddScoped<IMealPlanService, MealPlanService>();
builder.Services.AddScoped<IAccessService,AccessService>();
// HTTP clients
builder.Services.AddHttpContextAccessor();
 
//Database
var conn = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<PlanDbContext>(options=>options.UseNpgsql(conn));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("Health");

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();
