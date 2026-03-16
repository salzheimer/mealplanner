using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        return Ok(new { status = "ApiGateway is running" });
    }
}
