public record UserChanged
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public DateTimeOffset SourceUpdatedAt {get;init;}
    public string Action { get; init; } = string.Empty;  // Created, Updated, Deleted
}

public record GroupChanged
{
    public Guid GroupId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public DateTimeOffset SourceUpdatedAt {get;init;}
    public string Action { get; init; } = string.Empty;  // Created, Updated, Deleted
}

public record GroupMembershipChanged
{
    public Guid GroupMemberId {get; init;}
    public Guid UserId { get; init; }
    public Guid GroupId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public string StatusName { get; init; } = string.Empty;
    public DateTimeOffset SourceUpdatedAt {get;init;}
    public string Action { get; init; } = string.Empty;  // Added, Updated, Removed
}

public record CacheSyncRequested
{
    public string RequestedBy { get; init; } = string.Empty;  // triggers IdentityService to re-publish all current state
}