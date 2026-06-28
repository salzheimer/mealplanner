using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;

[Table("recipe_instructions")]
public class RecipeInstruction
{
    [Key]
    [Column("recipe_instruction_id")]
    public Guid Id { get; set; }
    [Column("recipe_id")]
    public Guid RecipeId { get; set; }
    [Column("step_number")]
    public int? StepNumber { get; set; }
    [Column("description")]
    public string? Description { get; set; }
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