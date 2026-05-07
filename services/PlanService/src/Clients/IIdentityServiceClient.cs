using Shared.Models;

namespace PlanService.Clients;

public interface IIdentityServiceClient
{
    Task<Result<ResourcePermissionDto>> GrantPermissionAsync(ResourcePermissionCreateDto permission);
    Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForResourceAsync(ResourceType resourceType, int resourceId);
    Task<Result<bool>> RevokePermissionAsync(long permissionId);
}
