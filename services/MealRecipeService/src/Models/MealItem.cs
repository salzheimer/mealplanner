using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MealRecipeService.Models;

[Table("meal_item")]
public class MealItem
{
    [Key]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("meal_id")]
    public int MealId { get; set; }
    [Column("recipe_id")]
    public int? RecipeId { get; set; }
    [Column("item_type")]
    public ItemType ItemType { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("created_by")]
    public int CreatedBy { get; set; }
    [Column("updated_by")]
    public int UpdatedBy { get; set; }
}



