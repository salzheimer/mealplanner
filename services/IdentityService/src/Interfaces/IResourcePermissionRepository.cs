using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IResourcePermissionRepository
{
    Task<ResourcePermission?> GetByIdAsync(long permissionId);
    Task<ResourcePermission?> GetPermissionAsync(ResourceType resourceType, int resourceId, SubjectType subjectType, int subjectId);
    Task<IEnumerable<ResourcePermission>> GetPermissionsForResourceAsync(ResourceType resourceType, int resourceId);
    Task<IEnumerable<ResourcePermission>> GetPermissionsForSubjectAsync(SubjectType subjectType, int subjectId);
    Task<ResourcePermission?> AddPermissionAsync(ResourcePermission permission);
    Task<bool> UpdatePermissionAsync(ResourcePermission permission);
    Task<bool> DeletePermissionAsync(long permissionId);
}