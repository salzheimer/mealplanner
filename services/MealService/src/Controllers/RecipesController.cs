using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MealService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public ActionResult<IEnumerable<RecipeDto>> GetAll()
    {
        // TODO: Replace with real data access
        return Ok(Array.Empty<RecipeDto>());
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public ActionResult<RecipeDto> GetById(int id)
    {
        // TODO: Replace with real data access
        return NotFound();
    }
}
