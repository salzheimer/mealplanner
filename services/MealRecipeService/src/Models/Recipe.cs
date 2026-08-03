using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;

[Table("recipes")]
public class Recipe
{
    [Key]
    [Column("recipe_id")]
    public Guid Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = null!;
    [Column("description")]
    public string? Description { get; set; }
    [Column("notes")]
    public string? Notes { get; set; }
    [Column("ranking")]
    public int? Ranking { get; set; }
    [Column("original_source")]
    public string? OriginalSource { get; set; }
    [Column("cook_time")]
    public TimeSpan? CookTime { get; set; }
    [Column("prep_time")]
    public TimeSpan? PrepTime { get; set; }
    [Column("servings")]
    public int? Servings { get; set; }
    [Column("owner_user_id")]
    public Guid? OwnerUserId { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [Column("created_by")]
    public Guid? CreatedBy { get; set; }
    [Column("updated_by")]
    public Guid? UpdatedBy { get; set; }
     
    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public List<RecipeInstruction> Instructions { get; set; } = new();
}

