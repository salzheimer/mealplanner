using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace PlanService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public ActionResult<IEnumerable<object>> GetAll()
    {
        // TODO: Replace with real data access
        return Ok(Array.Empty<object>());
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public ActionResult<object> GetById(int id)
    {
        // TODO: Replace with real data access
        return NotFound();
    }
}
