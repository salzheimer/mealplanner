using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models;

[Table("meal_item_plan_status_types")]
public class MealItemPlanStatusType
{
    [Key]
    [Column("meal_item_plan_status_type_id")]
    public int Id {get;set;}
    [Column("name")]
    public string Name {get;set;} = string.Empty;
    [Column("display_name")]
    public string DisplayName {get;set;} =string.Empty;
    [Column("sort_order")]
    public int SortOrder {get;set;}    
}