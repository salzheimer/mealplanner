using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
 

namespace PlanService.Controllers;

public class BaseController : ControllerBase
{
     internal Guid? GetAuthenticatedUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            // Map specific error types to appropriate HTTP status codes
            return result.Error.Type switch
            {
                ErrorType.Unauthorized => Unauthorized(new { error = result.Error }),
                ErrorType.NotFound => NotFound(new { error = result.Error }),
                ErrorType.InvalidInput => BadRequest(new { error = result.Error }),
                ErrorType.BadRequest => BadRequest(new { error = result.Error }),
                ErrorType.Unexpected => StatusCode(500, new { error = result.Error }),
                _ => StatusCode(500, new { error = "An unexpected error occurred." })
            };
        }
    }
}