using Shared.Models;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MealRecipeService.Clients;
using MealRecipeService.Repositories;
using MealRecipeService.Services;
using MealRecipeService.Interfaces;

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

// Repositories
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
builder.Services.AddScoped<IRecipeInstructionRepository, RecipeInstructionRepository>();
builder.Services.AddScoped<IMealRepository, MealRepository>();
builder.Services.AddScoped<IMealItemRepository, MealItemRepository>();
builder.Services.AddScoped<IMealTypeRepository, MealTypeRepository>();
builder.Services.AddScoped<IMealItemTypeRepository, MealItemTypeRepository>();
builder.Services.AddScoped<IResourcePermissionRepository, ResourcePermissionRepository>();
builder.Services.AddScoped<ICachedUserRepository, CachedUserRepository>();
builder.Services.AddScoped<ICachedGroupRepository, CachedGroupRepository>();
builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();
builder.Services.AddScoped<ISubjectTypeRepository, SubjectTypeRepository>();
builder.Services.AddScoped<IPermissionTypeRepository,PermissionTypeRepository>();
// Services
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IMealService, MealService>();
// HTTP clients
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IIdentityServiceClient, IdentityServiceClient>();

//Database
var conn = builder.Configuration.GetConnectionString("MealRecipe");
builder.Services.AddDbContext<MealRecipeDbContext>(options=>options.UseNpgsql(conn));


var app = builder.Build();

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
        options.WithTitle("Meal Recipe Service API");
        options.AddHttpAuthentication("Bearer", _ => { });
         
    });
}

app.Run();
