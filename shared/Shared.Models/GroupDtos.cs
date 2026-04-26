namespace Shared.Models;

public record GroupDto(
    int Id,
    string Name,
    int CreatedByUserId,
    DateTimeOffset CreatedAt
);

public record GroupCreateDto(
    string Name
);

public record GroupMemberDto(
    int Id,
    int UserId,
    int GroupId,
    GroupMemberRole Role,
    int? InvitedByUserId,
    DateTimeOffset? InvitedAt,
    DateTimeOffset? JoinedAt,
    GroupMemberStatus Status
);

