using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Services;
using IdentityService.Models;
using IdentityService.Interfaces;
using IdentityService.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using IdentityService.Contracts;

namespace IdentityService.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GroupController : BaseController
{
    private readonly IGroupService _groupService;
    
    public GroupController(IGroupService groupService)
    {
        _groupService =groupService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest createGroupRequest)
    {
        
        var authenticatedUserId = GetAuthenticatedUserId();
        if(authenticatedUserId ==null)
        {
            return HandleResult(Result<GroupResponse>.Failure(GroupErrors.Unauthorized));
        }
        //create group
        var newGroup = await _groupService.AddGroup(createGroupRequest);
        
        if(!newGroup.IsSuccess)
            return HandleResult(Result<GroupResponse>.Failure(GroupErrors.UnableToCreate));
        //assign group creator group owner role
        var groupOwner= new CreateGroupMemberRequest(GroupId:newGroup.Value.GroupId,UserId:authenticatedUserId.Value,GroupMemberRoleTypeName:"owner",GroupMemberStatusTypeName:"active");
        var ownerAdded= await _groupService.AddGroupMember(groupOwner);
        if(!ownerAdded.IsSuccess)
            return HandleResult(Result<GroupMemberResponse>.Failure(GroupMemberErrors.UnableToCreate));
        return HandleResult(Result<GroupResponse>.Success(newGroup.Value));
    }
}