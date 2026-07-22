namespace IdentityService.Contracts;


public record GroupMemberResponse(
    Guid GroupMemberId,
    Guid GroupId,
    string GroupName,
    Guid UserId,
    int GroupMemberRoleTypeId,
    string GroupMemberRoleTypeName,
    int GroupMemberStatusTypeId,
    string GroupMemberStatusTypeName,
    DateTimeOffset? InvitedAt,
    DateTimeOffset? JoinedAt,
    DateTimeOffset? RemovedAt
);
public record GroupMemberSummaryResponse(
    Guid GroupMemberId,
    Guid GroupId,
    string GroupName,
    Guid UserId,
    string? UserName,
    int GroupMemberRoleTypeId,
    string GroupMemberRoleTypeName,
    int GroupMemberStatusTypeId,
    string GroupMemberStatusTypeName
    
);
public record CreateGroupMemberRequest(
    Guid GroupId,
     
    Guid UserId,
    int RoleId,
    int StatusId
);
public record UpdateGroupMemberRequest(
    Guid GroupMemberId,
    Guid GroupId,
    Guid UserId,
    int GroupMemberRoleTypeId,
    int GroupMemberStatusTypeId,
    DateTimeOffset? InvitedAt,
    DateTimeOffset? JoinedAt,
    DateTimeOffset? RemovedAt
);