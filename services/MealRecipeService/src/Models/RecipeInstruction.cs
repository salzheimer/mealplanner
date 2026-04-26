using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;
[Table("recipe_instructions")]
public class RecipeInstruction
{
    [Key]
    public int Id { get; set; }
    [Column("recipe_id")]
    public int RecipeId { get; set; }
    [Column("step_number")] 
    public int? StepNumber { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    [Column("note")]
    public string? Note { get; set; }
}