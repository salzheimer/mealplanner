
namespace PlanService.Contracts;

public record CreateResourcePermissionRequest(
    string ResourceTypeName,
    Guid ResourceId,
    string SubjectTypeName,
    Guid SubjectId,
    string PermissionTypeName,
    Guid GrantedBy,
    DateTimeOffset? ExpiresAt
);

public record ResourcePermissionResponse(
    string? ResourceTypeName,
    int ResourceTypeId,
    Guid ResourceId,
    string? SubjectTypeName,
    int SubjectTypeId,
    string? PermissionTypeName,
    int PermissionTypeId,
    Guid SubjectId,
    Guid GrantedBy,
    DateTimeOffset? ExpiresAt
);
public record RevokeAccessRequest(
    Guid PermissionId,
    string ResourceTypeName,
    int? ResourceTypeId,
    Guid ResourceId,
    string SubjectTypeName,
    int? SubjectTypeId,    
    Guid SubjectId,
    Guid GrantedBy
);
/// <summary>
/// Groups get view access to resources so no need to include permission type 
/// </summary>
/// <param name="ResourceId"></param>
/// <param name="GroupId"></param>
/// <param name="GrantedBy"></param>
/// <param name="ExpiresAt"></param>
public record GroupGrantAccessRequest(
    Guid ResourceId,
    string ResourceTypeName,
    Guid GroupId,
    Guid GrantedBy,
    DateTimeOffset? ExpiresAt
);
public record UserGrantAccessRequest(
    Guid ResourceId,
    string ResourceTypeName,
    Guid UserId,
    Guid GrantedBy,
    int? PermissionTypeId,
    string PermissionTypeName,
    DateTimeOffset? ExpiresAt
);
public record GroupRevokeAccessRequest(
    string ResourceTypeName,
    Guid ResourceId,
    Guid GroupId
);
public record UserRevokeAccessRequest(
    string ResourceTypeName,
    Guid ResourceId,
    Guid UserId
);
