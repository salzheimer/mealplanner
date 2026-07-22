namespace IdentityService.Contracts;

public record GroupResponse(
    Guid GroupId,
    string Name,
    Guid CreatedBy,
    DateTimeOffset CreatedAt
);
public record GroupDetailResponse(
    Guid GroupId,
    string Name,
    string? OwnerDisplayName,
    Guid CreatedBy,
    DateTimeOffset CreatedAt
);
public record CreateGroupRequest(

    string Name
   

);
public record UpdateGroupRequest(
    Guid GroupId,
    string Name
     

);