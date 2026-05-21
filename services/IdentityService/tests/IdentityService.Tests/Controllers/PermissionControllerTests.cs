using IdentityService.Controllers;
using IdentityService.Models;
using IdentityService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Models;
using System.Security.Claims;
using Xunit;
using ResourceType = Shared.Models.ResourceType;
using SubjectType = Shared.Models.SubjectType;
using Permission = Shared.Models.Permission;

namespace IdentityService.Tests.Controllers;

public class PermissionControllerTests
{
    private readonly Mock<IResourcePermissionService> _permissionService;
    private readonly PermissionController _controller;
    private const int UserId = 1;

    public PermissionControllerTests()
    {
        _permissionService = new Mock<IResourcePermissionService>();
        _controller = new PermissionController(_permissionService.Object);
        SetAuthenticatedUser(_controller, UserId);
    }

    private static void SetAuthenticatedUser(ControllerBase controller, int userId)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
            }
        };
    }

    private static ResourcePermissionDto MakePermission(int resourceId = 1) => new(
        1L, ResourceType.Recipe, resourceId, SubjectType.User, 2, Permission.View, UserId
    );

    // --- GrantPermission ---

    [Fact]
    public async Task GrantPermission_ValidRequest_Returns200WithPermission()
    {
        var createDto = new ResourcePermissionCreateDto(ResourceType.Recipe, 1, SubjectType.User, 2, Permission.View, UserId, null);
        _permissionService.Setup(s => s.AddPermissionAsync(createDto))
            .ReturnsAsync(Result<ResourcePermissionDto>.Success(MakePermission(1)));

        var result = await _controller.GrantPermission(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ResourcePermissionDto>(ok.Value);
        Assert.Equal(ResourceType.Recipe, value.ResourceType);
    }

    [Fact]
    public async Task GrantPermission_ServiceFailure_Returns400()
    {
        var createDto = new ResourcePermissionCreateDto(ResourceType.Recipe, 1, SubjectType.User, 2, Permission.View, UserId, null);
        _permissionService.Setup(s => s.AddPermissionAsync(createDto))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(new Error("Permission.UnableToCreate", "Failed to create.", ErrorType.BadRequest)));

        var result = await _controller.GrantPermission(createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetResourcePermissions ---

    [Fact]
    public async Task GetResourcePermissions_ValidRequest_Returns200WithPermissions()
    {
        var permissions = new List<ResourcePermissionDto> { MakePermission(1) };
        _permissionService.Setup(s => s.GetPermissionsForResourceAsync(ResourceType.Recipe, 1))
            .ReturnsAsync(Result<IEnumerable<ResourcePermissionDto>>.Success(permissions));

        var result = await _controller.GetResourcePermissions("Recipe", 1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<ResourcePermissionDto>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetResourcePermissions_NoPermissions_Returns200WithEmptyList()
    {
        _permissionService.Setup(s => s.GetPermissionsForResourceAsync(ResourceType.Meal, 5))
            .ReturnsAsync(Result<IEnumerable<ResourcePermissionDto>>.Success(new List<ResourcePermissionDto>()));

        var result = await _controller.GetResourcePermissions("Meal", 5);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<ResourcePermissionDto>>(ok.Value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task GetResourcePermissions_InvalidResourceType_Returns400()
    {
        var result = await _controller.GetResourcePermissions("InvalidType", 1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetSubjectPermissions ---

    [Fact]
    public async Task GetSubjectPermissions_ValidRequest_Returns200WithPermissions()
    {
        var permissions = new List<ResourcePermissionDto> { MakePermission(1) };
        _permissionService.Setup(s => s.GetPermissionsForSubjectAsync(SubjectType.User, 2))
            .ReturnsAsync(Result<IEnumerable<ResourcePermissionDto>>.Success(permissions));

        var result = await _controller.GetSubjectPermissions("User", 2);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<ResourcePermissionDto>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetSubjectPermissions_InvalidSubjectType_Returns400()
    {
        var result = await _controller.GetSubjectPermissions("InvalidType", 1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- RevokePermission ---

    [Fact]
    public async Task RevokePermission_Granter_Returns200()
    {
        var permission = MakePermission(1);
        _permissionService.Setup(s => s.GetPermissionByIdAsync(1L))
            .ReturnsAsync(Result<ResourcePermissionDto?>.Success(permission));
        _permissionService.Setup(s => s.DeletePermissionAsync(1L))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.RevokePermission(1L);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task RevokePermission_PermissionNotFound_Returns404()
    {
        _permissionService.Setup(s => s.GetPermissionByIdAsync(999L))
            .ReturnsAsync(Result<ResourcePermissionDto?>.Success(null));

        var result = await _controller.RevokePermission(999L);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RevokePermission_NotGranter_Returns401()
    {
        var permission = new ResourcePermissionDto(1L, ResourceType.Recipe, 1, SubjectType.User, 2, Permission.View, GrantedBy: 99);
        _permissionService.Setup(s => s.GetPermissionByIdAsync(1L))
            .ReturnsAsync(Result<ResourcePermissionDto?>.Success(permission));

        var result = await _controller.RevokePermission(1L);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
