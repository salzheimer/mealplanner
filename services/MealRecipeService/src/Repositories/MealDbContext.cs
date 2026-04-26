using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class MealDbContext : DbContext
{
    public MealDbContext(DbContextOptions<MealDbContext> options) : base(options)
    {
    }

    public DbSet<MealRecipeService.Models.Meal> Meals { get; set; }
    public DbSet<MealRecipeService.Models.MealItem> MealItems { get; set; }
    public DbSet<MealRecipeService.Models.MealShare> MealShares { get; set; }
    public DbSet<MealRecipeService.Models.Recipe> Recipes { get; set; }
    public DbSet<MealRecipeService.Models.RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<MealRecipeService.Models.RecipeInstruction> RecipeInstructions { get; set; }
    public DbSet<MealRecipeService.Models.RecipeShare> RecipeShares { get; set; }
}