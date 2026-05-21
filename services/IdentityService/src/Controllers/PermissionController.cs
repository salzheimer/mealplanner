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
        return HandleResult(await _permissionService.AddPermissionAsync(permission));
    }

    [HttpGet("resource-permissions")]
    public async Task<IActionResult> GetResourcePermissions(string resourceType, int resourceId)
    {
        Shared.Models.ResourceType sharedResourceType;
        if (Enum.TryParse<Shared.Models.ResourceType>(resourceType, true, out var parsedResourceType))
        {
            sharedResourceType = parsedResourceType;
        }
        else
        {
            return HandleResult(Result<IEnumerable<ResourcePermissionDto>>.Failure(PermissionErrors.InvalidResourceType));
        }

        return HandleResult(await _permissionService.GetPermissionsForResourceAsync(sharedResourceType, resourceId));
    }

    [HttpGet("subject-permissions")]
    public async Task<IActionResult> GetSubjectPermissions(string subjectType, int subjectId)
    {
        Shared.Models.SubjectType sharedSubjectType;
        if (Enum.TryParse<Shared.Models.SubjectType>(subjectType, true, out var parsedSubjectType))
        {
            sharedSubjectType = parsedSubjectType;
        }
        else
        {
            return HandleResult(Result<IEnumerable<ResourcePermissionDto>>.Failure(PermissionErrors.InvalidSubjectType));
        }
        return HandleResult(await _permissionService.GetPermissionsForSubjectAsync(sharedSubjectType, subjectId));
    }

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