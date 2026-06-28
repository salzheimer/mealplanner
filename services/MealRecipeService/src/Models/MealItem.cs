using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MealRecipeService.Models;

[Table("meal_item")]
public class MealItem
{
    [Key]
    [Column("meal_item_id")]
    public Guid Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("meal_id")]
    public Guid MealId { get; set; }
    [Column("recipe_id")]
    public Guid? RecipeId { get; set; }
    [Column("item_type_id")]
    public int ItemTypeId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("created_by")]
    public Guid CreatedBy { get; set; }
    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    public MealItemType ItemType { get; set; } = null!;
}



