using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;
[Table("recipe_ingredients")]
public class RecipeIngredient
{
    [Key]
    [Column("recipe_ingredient_id")]
    public Guid Id { get; set; }
    [Column("recipe_id")]
    public Guid RecipeId { get; set; }
    [Column("name")]
    public string? Name { get; set; }
    [Column("amount")]
    public decimal? Amount { get; set; }
    [Column("measurement_type")]
    public string? MeasurementType { get; set; }
    [Column("note")]
     public string? Note { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("created_by")]
    public Guid CreatedBy { get; set; }
    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }
}