using Shared.Models;

namespace MealRecipeService.Clients;

public interface IIdentityServiceClient
{
    Task<Result<ResourcePermissionDto>> GrantPermissionAsync(ResourcePermissionCreateDto permission);
    Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForResourceAsync(ResourceType resourceType, int resourceId);
    Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForSubjectAsync(SubjectType subjectType, int subjectId);
    Task<Result<IEnumerable<ResourcePermissionDto>>> GetUserPermissionsAsync(int userId);
    Task<Result<bool>> RevokePermissionAsync(long permissionId);
}
