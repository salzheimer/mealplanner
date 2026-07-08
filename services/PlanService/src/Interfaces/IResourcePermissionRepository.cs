using PlanService.Models;

namespace PlanService.Interfaces;

public interface IResourcePermissionRepository
{
    Task<ResourcePermission?> GetByIdAsync(Guid id);
    Task<IEnumerable<ResourcePermission>> GetByResourceAsync(Guid resourceId, int resourceTypeId);
    Task<IEnumerable<ResourcePermission>> GetBySubjectAsync(Guid subjectId, int subjectTypeId);
    Task<IEnumerable<ResourcePermission>> GetBySubjectTypeAndResourceTypeAsync(Guid subjectId, int subjectTypeId, int resourceTypeId);
    Task<ResourcePermission?> GetBySubjectAndResourceAsync(Guid subjectId, int subjectTypeId, Guid resourceId, int resourceTypeId);
    Task<IEnumerable<ResourcePermission>> GetByGrantedByAsync(Guid GrantedById);
    Task<IEnumerable<ResourcePermission>> GetByResourceTypeNameAsync(Guid resourceId, string resourceTypeName);
    Task<IEnumerable<ResourcePermission>> GetBySubjectTypeNameAsync(Guid subjectId, string subjectTypeName);
    Task<IEnumerable<ResourcePermission>> GetBySubjectAndResourceTypeNamesAsync(Guid subjectId, string subjectTypeName, string resourceTypeName);
    Task<ResourcePermission?> GetBySubjectAndResourceByNamesAsync(Guid subjectId, string subjectTypeName, Guid resourceId, string resourceTypeName);
    Task<ResourcePermission?> CreateAsync(ResourcePermission permission);
    Task<bool> UpdateAsync(ResourcePermission permission);
    Task<bool> DeleteAsync(Guid id);
}
public interface IResourceTypeRepository
{
    Task<ResourceType?> GetByIdAsync(int id);
    Task<ResourceType?> GetByNameAsync(string name);
    Task<IEnumerable<ResourceType>> GetAllAsync();
    Task<ResourceType?> CreateAsync(ResourceType resourceType);
    Task<bool> UpdateAsync(ResourceType resourceType);
    Task<bool> DeleteAsync(int id);
}
public interface ISubjectTypeRepository
{
    Task<SubjectType?> GetByIdAsync(int id);
    Task<SubjectType?> GetByNameAsync(string name);
    Task<IEnumerable<SubjectType>> GetAllAsync();
    Task<SubjectType?> CreateAsync(SubjectType subjectType);
    Task<bool> UpdateAsync(SubjectType subjectType);
    Task<bool> DeleteAsync(int id);
}
public interface IPermissionTypeRepository
{
    Task<PermissionType?> GetByIdAsync(int id);
    Task<PermissionType?> GetByNameAsync(string name);
    Task<IEnumerable<PermissionType>> GetAllAsync();
    Task<PermissionType?> CreateAsync(PermissionType permissionType);
    Task<bool> UpdateAsync(PermissionType permissionType);
    Task<bool> DeleteAsync(int id);
}