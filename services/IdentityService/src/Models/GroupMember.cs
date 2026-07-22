using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models;

[Table("group_members")]
public class GroupMember
{
    [Key]
    [Column("group_member_id")]
    public Guid GroupMemberId { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("group_id")]
    public Guid GroupId { get; set; }
    [Column("role_id")]
    public int RoleId { get; set; }
    [Column("invited_by_user_id")]
    public Guid? InvitedByUserId { get; set; }
    [Column("invited_at")]
    public DateTimeOffset? InvitedAt { get; set; }
    [Column("joined_at")]
    public DateTimeOffset? JoinedAt { get; set; }
    [Column("removed_at")]
    public DateTimeOffset? RemovedAt { get; set; }
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
    [Column("status_id")]
    public int StatusId { get; set; }

    /// <summary>
    /// reference to parent
    /// </summary>
    [ForeignKey("GroupId")]
    public Group Group { get; set; } = null!;
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    [ForeignKey("RoleId")]
    public GroupMemberRoleType GroupMemberRoleType { get; set; } = null!;
    [ForeignKey("StatusId")]
    public GroupMemberStatusType GroupMemberStatusType { get; set; } = null!;
}