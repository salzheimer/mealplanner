
using Shared.Models;

namespace IdentityService.Interfaces
{
    public interface IResourcePermissionService
    {

        Task<Result<ResourcePermissionDto?>> GetPermissionByIdAsync(long permissionId);
    Task<Result<ResourcePermissionDto?>> GetPermissionAsync(int userId, ResourcePermissionDto permissionDto);
        Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForResourceAsync(Shared.Models.ResourceType resourceType, int resourceId);
        Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForSubjectAsync(Shared.Models.SubjectType subjectType, int subjectId);
        Task<Result<ResourcePermissionDto>> AddPermissionAsync(ResourcePermissionCreateDto permission);
        Task<Result<ResourcePermissionDto>> UpdatePermissionAsync(int userId, ResourcePermissionUpdateDto permission);
        Task<Result<bool>> DeletePermissionAsync(long permissionId);
    }
}