using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;
[Table("recipe_component")]
public class RecipeComponent
{
   [Key]
   [Column("recipe_component_id")]
   public Guid Id { get; set; }
   [Column("parent_recipe_id")]
   public Guid ParentRecipeId { get; set; }
   [Column("child_recipe_id")]
   public Guid ChildRecipeId { get; set; }
   [Column("sort_order")]
   public int SortOrder { get; set; }
   [Column("assembly_notes")]
   public string AssemblyNotes { get; set; }

}