using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Services;
using AuthService.Services;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserStore _userStore;
    private readonly JwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(UserStore userStore, JwtService jwtService, JwtSettings jwtSettings)
    {
        _userStore = userStore;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
    }

    [HttpPost("register")]
    public ActionResult<LoginResponse> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        if (_userStore.FindByUsername(request.Username) is not null)
        {
            return Conflict("User already exists.");
        }

        var user = _userStore.AddUser(request.Username, request.Password, request.Email);
        var token = _jwtService.GenerateToken(user.Id, user.Username, TimeSpan.FromMinutes(_jwtSettings.ExpiresMinutes));

        return Ok(new LoginResponse(token, "Bearer", _jwtSettings.ExpiresMinutes * 60));
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (!_userStore.ValidateCredentials(request.Username, request.Password))
        {
            return Unauthorized("Invalid credentials.");
        }

        var user = _userStore.FindByUsername(request.Username)!;
        var token = _jwtService.GenerateToken(user.Id, user.Username, TimeSpan.FromMinutes(_jwtSettings.ExpiresMinutes));

        return Ok(new LoginResponse(token, "Bearer", _jwtSettings.ExpiresMinutes * 60));
    }

    [HttpPost("validate")]
    public ActionResult Validate([FromBody] string token)
    {
        var principal = _jwtService.ValidateToken(token);
        if (principal is null)
        {
            return Unauthorized("Invalid token.");
        }

        return Ok(new { valid = true, claims = principal.Claims.Select(c => new { c.Type, c.Value }) });
    }
}
