using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;

[Table("resource_permissions")]
public class ResourcePermission
{
    [Key]
    [Column("resource_permission_id")]
    public Guid Id { get; set; }
    [Column("resource_type_id")]
    public int ResourceTypeId { get; set; }
    [Column("permission_type_id")]
    public int PermissionTypeId { get; set; }
    [Column("resource_id")]
    public Guid ResourceId { get; set; }
    [Column("subject_id")]
    public Guid SubjectId { get; set; }
    [Column("subject_type_id")]
    public int SubjectTypeId { get; set; }
    [Column("granted_by")]
    public Guid GrantedByUserId { get; set; }
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
    [Column("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    public PermissionType PermissionType { get; set; } = null!;
    public ResourceType ResourceType { get; set; } = null!;
    public SubjectType SubjectType { get; set; } = null!;
}