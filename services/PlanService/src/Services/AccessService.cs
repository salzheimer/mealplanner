

using PlanService.Contracts;
using PlanService.Interfaces;
using PlanService.Models;
using Shared.Models;

namespace PlanService.Services;

public class AccessService : Interfaces.IAccessService
{
    private readonly IResourcePermissionRepository _resourcePermissonsRepository;
    private readonly IResourceTypeRepository _resourceTypeRepository;
    private readonly ISubjectTypeRepository _subjectTypeRepository;
    private readonly IPermissionTypeRepository _permissionTypeRepository;


    public AccessService(IResourcePermissionRepository resourcePermissionRepository, IResourceTypeRepository resourceTypeRepository, ISubjectTypeRepository subjectTypeRepository, IPermissionTypeRepository permissionTypeRepository)
    {
        _resourcePermissonsRepository = resourcePermissionRepository;
        _resourceTypeRepository = resourceTypeRepository;
        _permissionTypeRepository = permissionTypeRepository;
        _subjectTypeRepository = subjectTypeRepository;
    }
   

    public async Task<Result<IEnumerable<ResourcePermissionResponse>>> GetSharedPlans(Guid planId)
    {
        var permissions = await _resourcePermissonsRepository.GetByResourceTypeNameAsync(planId, "plan");
        return Result<IEnumerable<ResourcePermissionResponse>>.Success(permissions.Select(ToResponse));
    }

    public async Task<Result<IEnumerable<ResourcePermissionResponse>>> GetPlansSharedWithUser(Guid userId)
    {
        var permissions = await _resourcePermissonsRepository.GetBySubjectAndResourceTypeNamesAsync(userId, "user", "plan");
        return Result<IEnumerable<ResourcePermissionResponse>>.Success(permissions.Select(ToResponse));
    }

    
    public async Task<Result<IEnumerable<ResourcePermissionResponse>>> GetResourcesSharedWithGroup(Guid groupId)
    {
        var permissions = await _resourcePermissonsRepository.GetBySubjectTypeNameAsync(groupId, "group");
        return Result<IEnumerable<ResourcePermissionResponse>>.Success(permissions.Select(ToResponse));
    }

    public async Task<Result<IEnumerable<ResourcePermissionResponse>>> GetResourcesSharedWithUser(Guid userId)
    {
        var permissions = await _resourcePermissonsRepository.GetBySubjectTypeNameAsync(userId, "user");
        return Result<IEnumerable<ResourcePermissionResponse>>.Success(permissions.Select(ToResponse));
    }

    public async Task<Result<IEnumerable<ResourcePermissionResponse>>> GetResourcesSharedByUser(Guid grantedById)
    {
        var permissions = await _resourcePermissonsRepository.GetByGrantedByAsync(grantedById);
        return Result<IEnumerable<ResourcePermissionResponse>>.Success(permissions.Select(ToResponse));
    }
    public async Task<Result<ResourcePermissionResponse>> GrantGroupPermissionToResource(GroupGrantAccessRequest groupGrantAccessRequest)
    {

        var subjectType = await GetSubjectType("group");
        var resourceType = await GetResourceType(groupGrantAccessRequest.ResourceTypeName);

        var permissionType = await GetPermissionType("view");
        if (subjectType.IsSuccess == false || resourceType.IsSuccess == false || permissionType.IsSuccess == false)
            return Result<ResourcePermissionResponse>.Failure(ResourcePermissionErrors.UnableToCreate);

        var resourcePermission = new ResourcePermission()
        {
            PermissionTypeId = permissionType.Value!.Id,
            ResourceTypeId = resourceType.Value!.Id,
            SubjectTypeId = subjectType.Value!.Id,
            ResourceId = groupGrantAccessRequest.ResourceId,
            SubjectId = groupGrantAccessRequest.GroupId,
            GrantedByUserId = groupGrantAccessRequest.GrantedBy,
            ExpiresAt = groupGrantAccessRequest.ExpiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var permissions = await _resourcePermissonsRepository.CreateAsync(resourcePermission);
        if (permissions == null)
            return Result<ResourcePermissionResponse>.Failure(ResourcePermissionErrors.UnableToCreate);
        var response = new ResourcePermissionResponse(resourceType.Value!.Name, permissions.ResourceTypeId, permissions.ResourceId, subjectType.Value!.Name, permissions.SubjectTypeId, permissionType.Value!.Name, permissions.PermissionTypeId, permissions.SubjectId, permissions.GrantedByUserId, permissions.ExpiresAt);

        return Result<ResourcePermissionResponse>.Success(response);

    }
    public async Task<Result<ResourcePermissionResponse>> GrantUserPermissionToResource(UserGrantAccessRequest userGrantAccessRequest)
    {

        var subjectType = await GetSubjectType("user");
        var resourceType = await GetResourceType(userGrantAccessRequest.ResourceTypeName);

        var permissionType = await GetPermissionType("view");
        if (subjectType.IsSuccess == false || resourceType.IsSuccess == false || permissionType.IsSuccess == false)
            return Result<ResourcePermissionResponse>.Failure(ResourcePermissionErrors.UnableToCreate);

        var resourcePermission = new ResourcePermission()
        {
            PermissionTypeId = permissionType.Value!.Id,
            ResourceTypeId = resourceType.Value!.Id,
            SubjectTypeId = subjectType.Value!.Id,
            ResourceId = userGrantAccessRequest.ResourceId,
            SubjectId = userGrantAccessRequest.UserId,
            GrantedByUserId = userGrantAccessRequest.GrantedBy,
            ExpiresAt = userGrantAccessRequest.ExpiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var permissions = await _resourcePermissonsRepository.CreateAsync(resourcePermission);
        if (permissions == null)
            return Result<ResourcePermissionResponse>.Failure(ResourcePermissionErrors.UnableToCreate);
        var response = new ResourcePermissionResponse(resourceType.Value!.Name, permissions.ResourceTypeId, permissions.ResourceId, subjectType.Value!.Name, permissions.SubjectTypeId, permissionType.Value!.Name, permissions.PermissionTypeId, permissions.SubjectId, permissions.GrantedByUserId, permissions.ExpiresAt);

        return Result<ResourcePermissionResponse>.Success(response);

    }


    public async Task<Result<ResourcePermissionResponse>> GrantAccessToResource(CreateResourcePermissionRequest permissionRequest)
    {
        var permissionType = await GetPermissionType(permissionRequest.PermissionTypeName);
        var resourceType = await GetResourceType(permissionRequest.ResourceTypeName);
        var subjectType = await GetSubjectType(permissionRequest.SubjectTypeName);
        if (permissionType.IsSuccess == false || resourceType.IsSuccess == false || subjectType.IsSuccess == false)
            return Result<ResourcePermissionResponse>.Failure(ResourcePermissionErrors.UnableToCreate);


        var resourcePermission = new ResourcePermission()
        {
            PermissionTypeId = permissionType.Value!.Id,
            GrantedByUserId = permissionRequest.GrantedBy,
            ResourceTypeId = resourceType.Value!.Id,
            ResourceId = permissionRequest.ResourceId,
            SubjectId = permissionRequest.SubjectId,
            SubjectTypeId = subjectType.Value!.Id,
            ExpiresAt = permissionRequest.ExpiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var permissions = await _resourcePermissonsRepository.CreateAsync(resourcePermission);
        if (permissions == null)
            return Result<ResourcePermissionResponse>.Failure(ResourcePermissionErrors.UnableToCreate);
        var response = new ResourcePermissionResponse(resourceType.Value!.Name, permissions.ResourceTypeId, permissions.ResourceId, subjectType.Value!.Name, permissions.SubjectTypeId, permissionType.Value!.Name, permissions.PermissionTypeId, permissions.SubjectId, permissions.GrantedByUserId, permissions.ExpiresAt);

        return Result<ResourcePermissionResponse>.Success(response);
    }

    public async Task<Result<bool>> RevokeGroupPermissionToResource(GroupRevokeAccessRequest revokeAccessRequest)
    {
        var resourcePermission = await _resourcePermissonsRepository.GetBySubjectAndResourceByNamesAsync(revokeAccessRequest.GroupId, "group", revokeAccessRequest.ResourceId, revokeAccessRequest.ResourceTypeName);
        if (resourcePermission == null)
            return Result<bool>.Failure(ResourcePermissionErrors.UnableToDelete);
        var isPermissionRemoved = await _resourcePermissonsRepository.DeleteAsync(resourcePermission.Id);
        return Result<bool>.Success(isPermissionRemoved);
    }

    public async Task<Result<bool>> RevokeUserPermissionToResource(UserRevokeAccessRequest revokeAccessRequest)
    {
        var resourcePermission = await _resourcePermissonsRepository.GetBySubjectAndResourceByNamesAsync(revokeAccessRequest.UserId, "user", revokeAccessRequest.ResourceId, revokeAccessRequest.ResourceTypeName);
        if (resourcePermission == null)
            return Result<bool>.Failure(ResourcePermissionErrors.UnableToDelete);
        var isPermissionRemoved = await _resourcePermissonsRepository.DeleteAsync(resourcePermission.Id);
        return Result<bool>.Success(isPermissionRemoved);
    }


    private async Task<Result<PermissionType>> GetPermissionType(string name)
    {
        var permissionType = await _permissionTypeRepository.GetByNameAsync(name);
        if (permissionType == null) return Result<PermissionType>.Failure(PermissionTypeErrors.InvalidType);
        return Result<PermissionType>.Success(permissionType);
    }
    private async Task<Result<SubjectType>> GetSubjectType(string name)
    {
        var subjectType = await _subjectTypeRepository.GetByNameAsync(name);
        if (subjectType == null) return Result<SubjectType>.Failure(SubjectTypeErrors.InvalidType);
        return Result<SubjectType>.Success(subjectType);
    }
    private static ResourcePermissionResponse ToResponse(ResourcePermission p) =>
        new(p.ResourceType.Name, p.ResourceTypeId, p.ResourceId,
            p.SubjectType.Name, p.SubjectTypeId,
            p.PermissionType.Name, p.PermissionTypeId,
            p.SubjectId, p.GrantedByUserId, p.ExpiresAt);

    private async Task<Result<ResourceType>> GetResourceType(string name)
    {
        var resourceType = await _resourceTypeRepository.GetByNameAsync(name);
        if (resourceType == null) return Result<ResourceType>.Failure(ResourceTypeErrors.InvalidType);
        return Result<ResourceType>.Success(resourceType);
    }
}