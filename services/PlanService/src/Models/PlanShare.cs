using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PlanService.Models;
[Table("plan_share")]
public class PlanShare
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("plan_id")]
    public int PlanId { get; set; }
    [Column("shared_with_user_id")]
    public int? SharedWithUserId { get; set; }
    [Column("shared_with_group_id")]
    public int? SharedWithGroupId { get; set; }
    [Column("shared_by_user_id")]
    public int SharedByUserId { get; set; }
    [Column("permission")]
    public Permission Permission { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; } 
}