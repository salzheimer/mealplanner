using IdentityService.Controllers;
using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IdentityService.Tests.Controllers;

public class LookupControllerTests
{
    private readonly Mock<IClientTypeRepository> _clientTypes;
    private readonly Mock<IGroupMemberRoleTypeRepository> _groupMemberRoles;
    private readonly Mock<IGroupMemberStatusTypeRepository> _groupMemberStatuses;
    private readonly LookupController _controller;

    public LookupControllerTests()
    {
        _clientTypes = new Mock<IClientTypeRepository>();
        _groupMemberRoles = new Mock<IGroupMemberRoleTypeRepository>();
        _groupMemberStatuses = new Mock<IGroupMemberStatusTypeRepository>();
        _controller = new LookupController(_clientTypes.Object, _groupMemberRoles.Object, _groupMemberStatuses.Object);
    }

    // --- Client Types ---

    [Fact]
    public async Task GetClientTypes_Success_Returns200WithItems()
    {
        var items = new List<ClientTypes> { new() { Id = 1, Name = "web", DisplayName = "Web", SortOrder = 1 } };
        _clientTypes.Setup(r => r.GetAllClientTypes()).ReturnsAsync(items);

        var result = await _controller.GetClientTypes();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<List<ClientTypes>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetClientTypes_NoneExist_Returns200WithEmptyList()
    {
        _clientTypes.Setup(r => r.GetAllClientTypes()).ReturnsAsync(new List<ClientTypes>());

        var result = await _controller.GetClientTypes();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<List<ClientTypes>>(ok.Value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task CreateClientType_ValidRequest_Returns201()
    {
        var request = new LookupItemRequest("web", "Web", 1);
        _clientTypes.Setup(r => r.CreateClientType(It.IsAny<ClientTypes>())).ReturnsAsync(1);

        var result = await _controller.CreateClientType(request);

        Assert.IsType<CreatedAtActionResult>(result);
        _clientTypes.Verify(r => r.CreateClientType(It.Is<ClientTypes>(c =>
            c.Name == "web" && c.DisplayName == "Web" && c.SortOrder == 1)), Times.Once);
    }

    [Fact]
    public async Task UpdateClientType_ExistingId_Returns200()
    {
        var request = new LookupItemRequest("web", "Web", 1);
        _clientTypes.Setup(r => r.UpdateClientType(It.IsAny<ClientTypes>())).ReturnsAsync(1);

        var result = await _controller.UpdateClientType(1, request);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateClientType_NonExistentId_Returns404()
    {
        var request = new LookupItemRequest("web", "Web", 1);
        _clientTypes.Setup(r => r.UpdateClientType(It.IsAny<ClientTypes>())).ReturnsAsync(0);

        var result = await _controller.UpdateClientType(99, request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteClientType_ExistingId_Returns200()
    {
        _clientTypes.Setup(r => r.DeleteClientType(1)).ReturnsAsync(1);

        var result = await _controller.DeleteClientType(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteClientType_NonExistentId_Returns404()
    {
        _clientTypes.Setup(r => r.DeleteClientType(99)).ReturnsAsync(0);

        var result = await _controller.DeleteClientType(99);

        Assert.IsType<NotFoundResult>(result);
    }

    // --- Group Member Roles ---

    [Fact]
    public async Task GetGroupMemberRoles_Success_Returns200WithItems()
    {
        var items = new List<GroupMemberRoleType> { new() { Id = 1, Name = "owner", DisplayName = "Owner", SortOrder = 1 } };
        _groupMemberRoles.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        var result = await _controller.GetGroupMemberRoles();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<List<GroupMemberRoleType>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetGroupMemberRoles_NoneExist_Returns200WithEmptyList()
    {
        _groupMemberRoles.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<GroupMemberRoleType>());

        var result = await _controller.GetGroupMemberRoles();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<List<GroupMemberRoleType>>(ok.Value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task CreateGroupMemberRole_ValidRequest_Returns201()
    {
        var request = new LookupItemRequest("owner", "Owner", 1);
        _groupMemberRoles.Setup(r => r.CreateAsync(It.IsAny<GroupMemberRoleType>())).ReturnsAsync(1);

        var result = await _controller.CreateGroupMemberRole(request);

        Assert.IsType<CreatedAtActionResult>(result);
        _groupMemberRoles.Verify(r => r.CreateAsync(It.Is<GroupMemberRoleType>(g =>
            g.Name == "owner" && g.DisplayName == "Owner" && g.SortOrder == 1)), Times.Once);
    }

    [Fact]
    public async Task UpdateGroupMemberRole_ExistingId_Returns200()
    {
        var request = new LookupItemRequest("owner", "Owner", 1);
        _groupMemberRoles.Setup(r => r.UpdateAsync(It.IsAny<GroupMemberRoleType>())).ReturnsAsync(1);

        var result = await _controller.UpdateGroupMemberRole(1, request);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateGroupMemberRole_NonExistentId_Returns404()
    {
        var request = new LookupItemRequest("owner", "Owner", 1);
        _groupMemberRoles.Setup(r => r.UpdateAsync(It.IsAny<GroupMemberRoleType>())).ReturnsAsync(0);

        var result = await _controller.UpdateGroupMemberRole(99, request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteGroupMemberRole_ExistingId_Returns200()
    {
        _groupMemberRoles.Setup(r => r.DeleteAsync(1)).ReturnsAsync(1);

        var result = await _controller.DeleteGroupMemberRole(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteGroupMemberRole_NonExistentId_Returns404()
    {
        _groupMemberRoles.Setup(r => r.DeleteAsync(99)).ReturnsAsync(0);

        var result = await _controller.DeleteGroupMemberRole(99);

        Assert.IsType<NotFoundResult>(result);
    }

    // --- Group Member Statuses ---

    [Fact]
    public async Task GetGroupMemberStatuses_Success_Returns200WithItems()
    {
        var items = new List<GroupMemberStatusType> { new() { Id = 1, Name = "active", DisplayName = "Active", SortOrder = 1 } };
        _groupMemberStatuses.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        var result = await _controller.GetGroupMemberStatuses();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<List<GroupMemberStatusType>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetGroupMemberStatuses_NoneExist_Returns200WithEmptyList()
    {
        _groupMemberStatuses.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<GroupMemberStatusType>());

        var result = await _controller.GetGroupMemberStatuses();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<List<GroupMemberStatusType>>(ok.Value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task CreateGroupMemberStatus_ValidRequest_Returns201()
    {
        var request = new LookupItemRequest("active", "Active", 1);
        _groupMemberStatuses.Setup(r => r.CreateAsync(It.IsAny<GroupMemberStatusType>())).ReturnsAsync(1);

        var result = await _controller.CreateGroupMemberStatus(request);

        Assert.IsType<CreatedAtActionResult>(result);
        _groupMemberStatuses.Verify(r => r.CreateAsync(It.Is<GroupMemberStatusType>(g =>
            g.Name == "active" && g.DisplayName == "Active" && g.SortOrder == 1)), Times.Once);
    }

    [Fact]
    public async Task UpdateGroupMemberStatus_ExistingId_Returns200()
    {
        var request = new LookupItemRequest("active", "Active", 1);
        _groupMemberStatuses.Setup(r => r.UpdateAsync(It.IsAny<GroupMemberStatusType>())).ReturnsAsync(1);

        var result = await _controller.UpdateGroupMemberStatus(1, request);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateGroupMemberStatus_NonExistentId_Returns404()
    {
        var request = new LookupItemRequest("active", "Active", 1);
        _groupMemberStatuses.Setup(r => r.UpdateAsync(It.IsAny<GroupMemberStatusType>())).ReturnsAsync(0);

        var result = await _controller.UpdateGroupMemberStatus(99, request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteGroupMemberStatus_ExistingId_Returns200()
    {
        _groupMemberStatuses.Setup(r => r.DeleteAsync(1)).ReturnsAsync(1);

        var result = await _controller.DeleteGroupMemberStatus(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteGroupMemberStatus_NonExistentId_Returns404()
    {
        _groupMemberStatuses.Setup(r => r.DeleteAsync(99)).ReturnsAsync(0);

        var result = await _controller.DeleteGroupMemberStatus(99);

        Assert.IsType<NotFoundResult>(result);
    }
}
