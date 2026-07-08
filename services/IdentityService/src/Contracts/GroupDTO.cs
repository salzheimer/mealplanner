namespace IdentityService.Contracts;

public record GroupResponse(
    Guid GroupId,
    string Name,
    Guid CreatedBy,
    DateTimeOffset CreatedAt
);

public record CreateGroupRequest(
    
    string Name,
    Guid CreatedBy
    
);
public record UpdateGroupRequest(
    Guid GroupId,
    string Name,
    Guid CreatedBy
    
);