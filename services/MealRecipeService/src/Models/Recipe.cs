using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;

[Table("recipe")]
public class Recipe
{
    [Key]
    public int Id { get; set; }
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
    public int? OwnerUserId { get; set; }
    [Column("visibility")]
    public Visibility Visibility { get; set; } = Visibility.Private;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public List<RecipeInstruction> Instructions { get; set; } = new();
}

