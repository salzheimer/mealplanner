using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MealRecipeService.Models;

[Table("item_types")]
public class MealItemType
{
   [Key]
   [Column("item_type_id")]
   public int Id { get; set; }
   [Column("name")]  
   public string Name { get; set; } = string.Empty;
   [Column("display_name")]
   public string DisplayName { get; set; } = string.Empty;
   [Column("sort_order")]
   public int SortOrder { get; set; }
}