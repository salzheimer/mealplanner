using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class MealRecipeDbContext : DbContext
{
    public MealRecipeDbContext(DbContextOptions<MealRecipeDbContext> options) : base(options)
    {
    }

    public DbSet<MealRecipeService.Models.Meal> Meals { get; set; }
    public DbSet<MealRecipeService.Models.MealItem> MealItems { get; set; }
    public DbSet<MealRecipeService.Models.ResourcePermission> ResourcePermissions { get; set; }
    public DbSet<MealRecipeService.Models.Recipe> Recipes { get; set; }
    public DbSet<MealRecipeService.Models.RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<MealRecipeService.Models.RecipeInstruction> RecipeInstructions { get; set; }
    public DbSet<MealRecipeService.Models.RecipeComponent> RecipeComponents { get; set; }

    // Lookup tables
public DbSet<MealRecipeService.Models.MealItemType> MealItemTypes { get; set; }
public DbSet<MealRecipeService.Models.MealType> MealTypes { get; set; }
public DbSet<MealRecipeService.Models.ResourceType> ResourceTypes { get; set; }
public DbSet<MealRecipeService.Models.SubjectType> SubjectTypes { get; set; }
public DbSet<MealRecipeService.Models.PermissionType> PermissionTypes { get; set; }

//Cache identity sets
public DbSet<MealRecipeService.Models.CachedUser> CachedUsers { get; set; }
public DbSet<MealRecipeService.Models.CachedGroup> CachedGroups { get; set; }
public DbSet<MealRecipeService.Models.CachedGroupMember> CachedGroupMembers { get; set; }
}