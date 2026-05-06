namespace   Shared.Models;

public record ResourcePermissionDto(
    long Id,
    ResourceType ResourceType,
    int ResourceId,
    SubjectType SubjectType,
    int SubjectId,
    Permission Permission,
    int GrantedBy
);
public record ResourcePermissionDetailDto(
    long Id,
    ResourceType ResourceType,
    int ResourceId,
    SubjectType SubjectType,
    int SubjectId,
    Permission Permission,
    int GrantedBy,
    DateTime GrantedAt,
    DateTime? ExpiresAt,
    DateTime CreatedAt
);
public record ResourcePermissionCreateDto(
    ResourceType ResourceType,
    int ResourceId,
    SubjectType SubjectType,
    int SubjectId,
    Permission Permission,
    int GrantedBy,
    DateTime? ExpiresAt
);

public record ResourcePermissionUpdateDto(
    long Id,
    Permission Permission,
    SubjectType SubjectType,
    int SubjectId,
    DateTime? ExpiresAt
);

public record ShareRequestDto(
    SubjectType SubjectType,
    int SubjectId,
    Permission Permission,
    DateTime? ExpiresAt
);