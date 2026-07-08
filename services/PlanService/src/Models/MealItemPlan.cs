using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models;
[Table("plan_meal_items")]
public class PlanMealItem
{
    [Key]
    [Column("plan_meal_item_id")]
    public Guid Id { get; set; }
    [Column("meal_plan_id")]
    public Guid MealPlanId { get; set; }
    [Column("meal_item_id")]
    public Guid MealItemId { get; set; }
   
    [Column("assigned_to_guest_name")]  
    public string? AssignedToGuestName { get; set; } = string.Empty;
    [Column("assigned_to_user")]
    public Guid? AssignedToUserId { get; set; }
    [Column("status_id")]
    public int StatusId { get; set; }
    [Column("notes")]
    public string Notes { get; set; } = string.Empty;   
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")]  
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [Column("created_by")]
    public Guid CreatedBy { get; set; }
    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }

    public MealItemPlanStatusType MealItemPlanStatusType {get;set;} =null!;
}
