using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Interfaces;
using IdentityService.Models;

namespace IdentityService.Controllers;

public record LookupItemRequest(string Name, string DisplayName, int SortOrder);

[ApiController]
[Route("api/lookup")]
[Authorize(Policy = "InternalServiceOnly")]
public class LookupController : BaseController
{
    private readonly IClientTypeRepository _clientTypes;
    private readonly IGroupMemberRoleTypeRepository _groupMemberRoles;
    private readonly IGroupMemberStatusTypeRepository _groupMemberStatuses;

    public LookupController(
        IClientTypeRepository clientTypes,
        IGroupMemberRoleTypeRepository groupMemberRoles,
        IGroupMemberStatusTypeRepository groupMemberStatuses)
    {
        _clientTypes = clientTypes;
        _groupMemberRoles = groupMemberRoles;
        _groupMemberStatuses = groupMemberStatuses;
    }

    // --- Client Types ---

    [HttpGet("client-types")]
    public async Task<IActionResult> GetClientTypes() =>
        Ok(await _clientTypes.GetAllClientTypes());

    [HttpPost("client-types")]
    public async Task<IActionResult> CreateClientType([FromBody] LookupItemRequest request)
    {
        var item = new ClientTypes { Name = request.Name, DisplayName = request.DisplayName, SortOrder = request.SortOrder };
        await _clientTypes.CreateClientType(item);
        return CreatedAtAction(nameof(GetClientTypes), null);
    }

    [HttpPut("client-types/{id:int}")]
    public async Task<IActionResult> UpdateClientType(int id, [FromBody] LookupItemRequest request)
    {
        var item = new ClientTypes { Id = id, Name = request.Name, DisplayName = request.DisplayName, SortOrder = request.SortOrder };
        var affected = await _clientTypes.UpdateClientType(item);
        return affected == 0 ? NotFound() : Ok();
    }

    [HttpDelete("client-types/{id:int}")]
    public async Task<IActionResult> DeleteClientType(int id)
    {
        var affected = await _clientTypes.DeleteClientType(id);
        return affected == 0 ? NotFound() : Ok();
    }

    // --- Group Member Roles ---

    [HttpGet("group-member-roles")]
    public async Task<IActionResult> GetGroupMemberRoles() =>
        Ok(await _groupMemberRoles.GetAllAsync());

    [HttpPost("group-member-roles")]
    public async Task<IActionResult> CreateGroupMemberRole([FromBody] LookupItemRequest request)
    {
        var item = new GroupMemberRoleType { Name = request.Name, DisplayName = request.DisplayName, SortOrder = request.SortOrder };
        await _groupMemberRoles.CreateAsync(item);
        return CreatedAtAction(nameof(GetGroupMemberRoles), null);
    }

    [HttpPut("group-member-roles/{id:int}")]
    public async Task<IActionResult> UpdateGroupMemberRole(int id, [FromBody] LookupItemRequest request)
    {
        var item = new GroupMemberRoleType { Id = id, Name = request.Name, DisplayName = request.DisplayName, SortOrder = request.SortOrder };
        var affected = await _groupMemberRoles.UpdateAsync(item);
        return affected == 0 ? NotFound() : Ok();
    }

    [HttpDelete("group-member-roles/{id:int}")]
    public async Task<IActionResult> DeleteGroupMemberRole(int id)
    {
        var affected = await _groupMemberRoles.DeleteAsync(id);
        return affected == 0 ? NotFound() : Ok();
    }

    // --- Group Member Statuses ---

    [HttpGet("group-member-statuses")]
    public async Task<IActionResult> GetGroupMemberStatuses() =>
        Ok(await _groupMemberStatuses.GetAllAsync());

    [HttpPost("group-member-statuses")]
    public async Task<IActionResult> CreateGroupMemberStatus([FromBody] LookupItemRequest request)
    {
        var item = new GroupMemberStatusType { Name = request.Name, DisplayName = request.DisplayName, SortOrder = request.SortOrder };
        await _groupMemberStatuses.CreateAsync(item);
        return CreatedAtAction(nameof(GetGroupMemberStatuses), null);
    }

    [HttpPut("group-member-statuses/{id:int}")]
    public async Task<IActionResult> UpdateGroupMemberStatus(int id, [FromBody] LookupItemRequest request)
    {
        var item = new GroupMemberStatusType { Id = id, Name = request.Name, DisplayName = request.DisplayName, SortOrder = request.SortOrder };
        var affected = await _groupMemberStatuses.UpdateAsync(item);
        return affected == 0 ? NotFound() : Ok();
    }

    [HttpDelete("group-member-statuses/{id:int}")]
    public async Task<IActionResult> DeleteGroupMemberStatus(int id)
    {
        var affected = await _groupMemberStatuses.DeleteAsync(id);
        return affected == 0 ? NotFound() : Ok();
    }
}
