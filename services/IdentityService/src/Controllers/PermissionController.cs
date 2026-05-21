using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Services;
using IdentityService.Models;
using IdentityService.Interfaces;

namespace IdentityService.Controllers;

[ApiController]
[Authorize(Policy = "InternalServiceOnly")]
[Route("api/[controller]")]
public class PermissionController : BaseController
{
    private readonly IResourcePermissionService _permissionService;

    public PermissionController(IResourcePermissionService resourcePermissionService)
    {
        _permissionService = resourcePermissionService;
    }

    [HttpPost]
    public async Task<IActionResult> GrantPermission([FromBody] ResourcePermissionCreateDto permission)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<ResourcePermissionDto>.Failure(PermissionErrors.Unauthorized));
        }
        var result = HandleResult(await _permissionService.AddPermissionAsync(permission));
        return result;
    }
    [HttpGet("resource-permissions")]
    public async Task<IActionResult> GetResourcePermissions(string resourceType, int resourceId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<ResourcePermissionDto>>.Failure(PermissionErrors.Unauthorized));
        }
        Shared.Models.ResourceType sharedResourceType;
        if (Enum.TryParse<Shared.Models.ResourceType>(resourceType, true, out var parsedResourceType))
        {
            sharedResourceType = parsedResourceType;
        }
        else
        {
            return HandleResult(Result<IEnumerable<ResourcePermissionDto>>.Failure(PermissionErrors.InvalidResourceType));
        }

        var permissions = HandleResult(await _permissionService.GetPermissionsForResourceAsync(sharedResourceType, resourceId));
        return permissions;
    }
    [HttpGet("subject-permissions")]
    public async Task<IActionResult> GetSubjectPermissions(string subjectType, int subjectId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<ResourcePermissionDto>>.Failure(PermissionErrors.Unauthorized));
        }
        Shared.Models.SubjectType sharedSubjectType;
        if (Enum.TryParse<Shared.Models.SubjectType>(subjectType, true, out var parsedSubjectType))
        {
            sharedSubjectType = parsedSubjectType;
        }
        else
        {
            return HandleResult(Result<IEnumerable<ResourcePermissionDto>>.Failure(PermissionErrors.InvalidSubjectType));
        }
        var permissions = HandleResult(await _permissionService.GetPermissionsForSubjectAsync(sharedSubjectType, subjectId));
        return permissions;
    }
    // [HttpGet("user-permissions")]
    // [Authorize]
    // public async Task<IActionResult> GetUserPermissions(int userId)
    // {
    //     var authenticatedUserId = GetAuthenticatedUserId();
    //     if (authenticatedUserId == null)
    //     {
    //         return HandleResult(Result<IEnumerable<ResourcePermissionDto>>.Failure(UserErrors.Unauthorized));
    //     }

    //     var permissions = HandleResult(await _permissionService.GetPermissionsForSubjectAsync(Shared.Models.SubjectType.User, userId));
    //     return permissions;
    // }

    [HttpDelete("{permissionId:long}")]
    public async Task<IActionResult> RevokePermission(long permissionId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
            return HandleResult(Result<bool>.Failure(PermissionErrors.Unauthorized));

        var existing = await _permissionService.GetPermissionByIdAsync(permissionId);
        if (!existing.IsSuccess || existing.Value == null)
            return HandleResult(Result<bool>.Failure(PermissionErrors.NotFound));

         // Only the user that grant access or the user granted the permssion can remove access.
        if (existing.Value.GrantedBy != authenticatedUserId || (existing.Value.SubjectType == Shared.Models.SubjectType.User && existing.Value.SubjectId == authenticatedUserId))
            return HandleResult(Result<bool>.Failure(PermissionErrors.Unauthorized));

        return HandleResult(await _permissionService.DeletePermissionAsync(permissionId));
    }

}