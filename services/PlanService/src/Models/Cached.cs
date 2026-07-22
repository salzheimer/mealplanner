using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models;

[Table("cached_users")]
public class CachedUser
{
    [Key]
    [Column("user_id")]
    public Guid Id { get; set; }
    [Column("display_name")]
    public string DisplayName { get; set; } = null!;
    [Column("synced_at")]
    public DateTimeOffset SyncedAt { get; set; }
}

[Table("cached_groups")]
public class CachedGroup
{
    [Key]
    [Column("group_id")]
    public Guid Id { get; set; }
    [Column("group_name")]
    public string GroupName { get; set; } = null!;
    [Column("synced_at")]
    public DateTimeOffset SyncedAt { get; set; }
    [Column("source_updated_at")]
    public DateTimeOffset SourceUpdatedAt {get;set;}
}

[Table("cached_group_members")]
public class CachedGroupMember
{
    [Key]
    [Column("group_member_id")]
    public Guid Id { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("group_id")]
    public Guid GroupId { get; set; }
   
    [Column("role_name")]
    public string RoleName { get; set; } =null!;
    [Column("status_name")]
    public string StatusName { get; set; } =null!;
    [Column("synced_at")]
    public DateTimeOffset SyncedAt { get; set; }
    [Column("source_updated_at")]
    public DateTimeOffset SourceUpdatedAt {get;set;}
}