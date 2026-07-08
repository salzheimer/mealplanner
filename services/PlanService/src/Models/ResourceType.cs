using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models;
[Table("resource_types")]
public class ResourceType
{
   [Key]
   [Column("resource_type_id")]
   public int Id { get; set; }
   [Column("name")]  
   public string Name { get; set; } = string.Empty;
   [Column("display_name")]
   public string DisplayName { get; set; } = string.Empty;
   [Column("sort_order")]
   public int SortOrder { get; set; }
}