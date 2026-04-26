using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;
[Table("recipe_ingredients")]
public class RecipeIngredient
{
    [Key]
    public int Id { get; set; }
    [Column("recipe_id")]
    public int RecipeId { get; set; }
    [Column("name")]
    public string? Name { get; set; }
    [Column("amount")]
    public decimal? Amount { get; set; }
    [Column("measurement_type")]
    public string? MeasurementType { get; set; }
}