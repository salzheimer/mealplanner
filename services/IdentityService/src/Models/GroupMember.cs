using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models;

[Table("group_members")]
public class GroupMember
{
    [Key]
    [Column("group_member_id")]
    public Guid GroupMemberId{get;set;}
    [Column("user_id")]
    public Guid UserId {get;set;}
    [Column("group_id")]
    public Guid GroupId {get;set;}
    [Column("role_id")]
    public int RoleId{get;set;}
    [Column("invited_by_user_id")]
    public Guid? InvitedByUserId{get;set;}
    [Column("invited_at")]
    public DateTimeOffset? InvitedAt{get;set;}
    [Column("joined_at")]
    public DateTimeOffset? JoinedAt{get;set;}
    [Column("removed_at")]
    public DateTimeOffset? RemovedAt{get;set;}
    
    [Column("status_id")]
    public int StatusId {get;set;}
    
    /// <summary>
    /// reference to parent
    /// </summary>
    public Group Group {get;set;} =null!;
    public GroupMemberRoleType GroupMemberRoleType {get;set;}= null!;
    public GroupMemberStatusType GroupMemberStatusType {get;set;}= null!;
}