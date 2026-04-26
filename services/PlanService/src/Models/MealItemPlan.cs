using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models;
[Table("meal_item")]
public class MealItemPlan
{
    [Key]
    public int Id { get; set; }
    [Column("meal_plan_id")]
    public int MealPlanId { get; set; }
    [Column("meal_item_id")]
    public int MealItemId { get; set; }
   
    [Column("assigned_to_guest_name")]  
    public string AssignedToGuestName { get; set; } = string.Empty;
    [Column("assigned_to_user")]
    public int AssignedToUserId { get; set; }
    [Column("status")]
    public ItemStatus Status { get; set; }
    [Column("notes")]
    public string Notes { get; set; } = string.Empty;   
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")]  
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


}
