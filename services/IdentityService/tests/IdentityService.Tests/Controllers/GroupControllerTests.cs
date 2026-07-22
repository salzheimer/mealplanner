using IdentityService.Contracts;
using IdentityService.Controllers;
using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Models;
using System.Security.Claims;
using Xunit;

namespace IdentityService.Tests.Controllers;

public class GroupControllerTests
{
    private readonly Mock<IGroupService> _groupService;
    private readonly GroupController _controller;
    private static readonly Guid UserId = Guid.NewGuid();

    public GroupControllerTests()
    {
        _groupService = new Mock<IGroupService>();
        _controller = new GroupController(_groupService.Object);
        SetAuthenticatedUser(_controller, UserId);
    }

    private static void SetAuthenticatedUser(ControllerBase controller, Guid userId)
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

    private static GroupResponse MakeGroup(Guid? groupId = null) => new(
        groupId ?? Guid.NewGuid(), "Test Group", UserId, DateTimeOffset.UtcNow
    );

    // --- CreateGroup ---
    // GroupService.AddGroup owns the full creation flow (group row + owner membership + event
    // publishing) internally, so the controller only ever sees a single Result<GroupResponse>.

    [Fact]
    public async Task CreateGroup_ValidRequest_Returns200WithGroup()
    {
        var groupId = Guid.NewGuid();
        var createDto = new CreateGroupRequest("Test Group");
        _groupService.Setup(s => s.AddGroup(UserId, createDto))
            .ReturnsAsync(Result<GroupResponse>.Success(MakeGroup(groupId)));

        var result = await _controller.CreateGroup(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GroupResponse>(ok.Value);
        Assert.Equal(groupId, value.GroupId);
    }

    [Fact]
    public async Task CreateGroup_ServiceFailure_Returns500()
    {
        var createDto = new CreateGroupRequest("Test Group");
        _groupService.Setup(s => s.AddGroup(UserId, createDto))
            .ReturnsAsync(Result<GroupResponse>.Failure(GroupErrors.UnableToCreate));

        var result = await _controller.CreateGroup(createDto);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    // --- MyGroups ---

    [Fact]
    public async Task MyGroups_Success_Returns200WithGroups()
    {
        var groups = new List<GroupResponse> { MakeGroup(), MakeGroup() };
        _groupService.Setup(s => s.GetGroupsUserBelongs(UserId, UserId))
            .ReturnsAsync(Result<IEnumerable<GroupResponse>>.Success(groups));

        var result = await _controller.MyGroups();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<GroupResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task MyGroups_ServiceFailure_Returns404()
    {
        _groupService.Setup(s => s.GetGroupsUserBelongs(UserId, UserId))
            .ReturnsAsync(Result<IEnumerable<GroupResponse>>.Failure(GroupErrors.NotFound));

        var result = await _controller.MyGroups();

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
