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

    private static GroupMemberResponse MakeGroupMember(Guid groupId) => new(
        Guid.NewGuid(), groupId, "Test Group", UserId, 1, "owner", 1, "active", null, DateTimeOffset.UtcNow, null
    );

    // --- CreateGroup ---

    [Fact]
    public async Task CreateGroup_ValidRequest_Returns200WithGroup()
    {
        var groupId = Guid.NewGuid();
        var createDto = new CreateGroupRequest("Test Group");
        _groupService.Setup(s => s.AddGroup(UserId,createDto))
            .ReturnsAsync(Result<GroupResponse>.Success(MakeGroup(groupId)));
        _groupService.Setup(s => s.AddGroupMember(UserId,It.IsAny<CreateGroupMemberRequest>()))
            .ReturnsAsync(Result<GroupMemberResponse>.Success(MakeGroupMember(groupId)));

        var result = await _controller.CreateGroup(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GroupResponse>(ok.Value);
        Assert.Equal(groupId, value.GroupId);
    }

    [Fact]
    public async Task CreateGroup_GroupCreationFails_Returns500()
    {
        var createDto = new CreateGroupRequest("Test Group");
        _groupService.Setup(s => s.AddGroup(UserId,createDto))
            .ReturnsAsync(Result<GroupResponse>.Failure(GroupErrors.UnableToCreate));

        var result = await _controller.CreateGroup(createDto);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_OwnerAssignmentFails_Returns500()
    {
        var groupId = Guid.NewGuid();
        var createDto = new CreateGroupRequest("Test Group");
        _groupService.Setup(s => s.AddGroup(UserId,createDto))
            .ReturnsAsync(Result<GroupResponse>.Success(MakeGroup(groupId)));
        _groupService.Setup(s => s.AddGroupMember(UserId,It.IsAny<CreateGroupMemberRequest>()))
            .ReturnsAsync(Result<GroupMemberResponse>.Failure(GroupMemberErrors.UnableToCreate));

        var result = await _controller.CreateGroup(createDto);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }
}
