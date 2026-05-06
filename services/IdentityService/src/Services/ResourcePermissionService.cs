using IdentityService.Models;
using IdentityService.Interfaces;
using Shared.Models;
using IdentityService.Mappings;

namespace IdentityService.Services;

public class ResourcePermissionService : IResourcePermissionService
{
    private readonly IResourcePermissionRepository _repository;

    public ResourcePermissionService(IResourcePermissionRepository repository)
    {
        _repository = repository;
    }



    public async Task<Result<ResourcePermissionDto>> AddPermissionAsync(ResourcePermissionCreateDto permission)
    {

        var entity = new ResourcePermission
        {
            ResourceType = EnumMappings.ToEntityResourceType(permission.ResourceType),
            ResourceId = permission.ResourceId,
            SubjectType = EnumMappings.ToEntitySubjectType(permission.SubjectType),
            SubjectId = permission.SubjectId,
            Permission = EnumMappings.ToEntityPermission(permission.Permission),
            GrantedBy = permission.GrantedBy,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = permission.ExpiresAt,
            UpdatedAt = DateTime.Now,
            UpdatedBy = permission.GrantedBy

        };

        var createdPermission = await _repository.AddPermissionAsync(entity);

        if (createdPermission == null)
        {
            return Result<ResourcePermissionDto>.Failure(new Error("GrantAccess.Failed", $"Failed to create access to resource {permission.ResourceType} with id {permission.ResourceId} for subject {permission.SubjectType} with id {permission.SubjectId}"));
        }
        var resultDto = new ResourcePermissionDto(
            createdPermission.Id,
            EnumMappings.ToDtoResourceType(createdPermission.ResourceType),
            createdPermission.ResourceId,
            EnumMappings.ToDtoSubjectType(createdPermission.SubjectType),
            createdPermission.SubjectId,
            EnumMappings.ToDtoPermission(createdPermission.Permission),
            createdPermission.GrantedBy

        );
        return Result<ResourcePermissionDto>.Success(resultDto);
    }

    public async Task<Result<ResourcePermissionDto>> UpdatePermissionAsync(int userId, ResourcePermissionUpdateDto permission)
    {
        var entity = new ResourcePermission
        {
            Id = permission.Id,
            Permission = EnumMappings.ToEntityPermission(permission.Permission),
            ExpiresAt = permission.ExpiresAt,
            SubjectType = EnumMappings.ToEntitySubjectType(permission.SubjectType),
            SubjectId = permission.SubjectId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };
        var updatedPermissions = await _repository.UpdatePermissionAsync(entity);
        if (!updatedPermissions)
        {
            return Result<ResourcePermissionDto>.Failure(new Error("UpdatePermission.Failed", $"Failed to update permission with id {permission.Id}"));
        }
        var resultDto = new ResourcePermissionDto(
            Id: entity.Id,
            ResourceType: EnumMappings.ToDtoResourceType(entity.ResourceType),
            ResourceId: entity.ResourceId,
            SubjectType: EnumMappings.ToDtoSubjectType(entity.SubjectType),
            SubjectId: entity.SubjectId,
            Permission: EnumMappings.ToDtoPermission(entity.Permission),
            GrantedBy: entity.GrantedBy
        );
        return Result<ResourcePermissionDto>.Success(resultDto);
    }

    public async Task<Result<bool>> DeletePermissionAsync(long permissionId)
    {
        var result = await _repository.DeletePermissionAsync(permissionId);
        if (!result)
        {
            return Result<bool>.Failure(new Error("DeletePermission.Failed", $"Failed to delete permission with id {permissionId}"));
        }
        return Result<bool>.Success(true);
    }

    public async Task<Result<ResourcePermissionDto?>> GetPermissionByIdAsync(long permissionId)
    {
        var permission = await _repository.GetByIdAsync(permissionId);
        if (permission == null)
            return Result<ResourcePermissionDto?>.Success(null);
        var dto = new ResourcePermissionDto(
            Id: permission.Id,
            ResourceType: EnumMappings.ToDtoResourceType(permission.ResourceType),
            ResourceId: permission.ResourceId,
            SubjectType: EnumMappings.ToDtoSubjectType(permission.SubjectType),
            SubjectId: permission.SubjectId,
            Permission: EnumMappings.ToDtoPermission(permission.Permission),
            GrantedBy: permission.GrantedBy
        );
        return Result<ResourcePermissionDto?>.Success(dto);
    }

    public async Task<Result<ResourcePermissionDto?>> GetPermissionAsync(int userId, ResourcePermissionDto permissionDto)
    {
        var permission = await _repository.GetPermissionAsync(
            EnumMappings.ToEntityResourceType(permissionDto.ResourceType),
            permissionDto.ResourceId,
            EnumMappings.ToEntitySubjectType(permissionDto.SubjectType),
            permissionDto.SubjectId
        );
        if (permission == null)
        {
            return Result<ResourcePermissionDto?>.Success(null);
        }
        var resultDto = new ResourcePermissionDto(
            Id: permission.Id,
            ResourceType: EnumMappings.ToDtoResourceType(permission.ResourceType),
            ResourceId: permission.ResourceId,
            SubjectType: EnumMappings.ToDtoSubjectType(permission.SubjectType),
            SubjectId: permission.SubjectId,
            Permission: EnumMappings.ToDtoPermission(permission.Permission),
            GrantedBy: permission.GrantedBy
        );
        return Result<ResourcePermissionDto?>.Success(resultDto);
    }

    public async Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForResourceAsync(Shared.Models.ResourceType resourceType, int resourceId)
    {
        var permissions = await _repository.GetPermissionsForResourceAsync(EnumMappings.ToEntityResourceType(resourceType), resourceId);
        var resultDtos = permissions.Select(p => new ResourcePermissionDto(
            Id: p.Id,
            ResourceType: EnumMappings.ToDtoResourceType(p.ResourceType),
            ResourceId: p.ResourceId,
            SubjectType: EnumMappings.ToDtoSubjectType(p.SubjectType),
            SubjectId: p.SubjectId,
            Permission: EnumMappings.ToDtoPermission(p.Permission),
            GrantedBy: p.GrantedBy
        ));
        return Result<IEnumerable<ResourcePermissionDto>>.Success(resultDtos);
    }

    public async Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForSubjectAsync(Shared.Models.SubjectType subjectType, int subjectId)
    {
        var permissions = await _repository.GetPermissionsForSubjectAsync(EnumMappings.ToEntitySubjectType(subjectType), subjectId);
        var resultDtos = permissions.Select(p => new ResourcePermissionDto(
            Id: p.Id,
            ResourceType: EnumMappings.ToDtoResourceType(p.ResourceType),
            ResourceId: p.ResourceId,
            SubjectType: EnumMappings.ToDtoSubjectType(p.SubjectType),
            SubjectId: p.SubjectId,
            Permission: EnumMappings.ToDtoPermission(p.Permission),
            GrantedBy: p.GrantedBy
        ));
        return Result<IEnumerable<ResourcePermissionDto>>.Success(resultDtos);
    }
        


}