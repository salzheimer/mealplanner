using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace IdentityService.Models;

[Table("resource_permissions")]
public class ResourcePermission
{
[Key]
    public long Id { get; set; }
    [Column("resource_type", TypeName = "resource_type_enum")]
    public ResourceType ResourceType { get; set; }
    [Column("resource_id")]
    public int ResourceId { get; set; }
    [Column("subject_type", TypeName = "subject_type_enum")]
    public SubjectType SubjectType { get; set; }
    [Column("subject_id")]
    public int SubjectId { get; set; }
    [Column("permission", TypeName = "permission_enum")]
    public Permission Permission { get; set; }
    [Column("granted_by")]
    public int GrantedBy { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [Column("updated_by")]
    public int? UpdatedBy { get; set; }
}