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
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        if (_userStore.FindByEmail(request.Email) is not null)
        {
            return Conflict("User already exists.");
        }

        var user = _userStore.AddUser(request.Email, request.Password, request.DisplayName);
        var token = _jwtService.GenerateToken(user.Id, user.Email, TimeSpan.FromMinutes(_jwtSettings.ExpiresMinutes));

        return Ok(new LoginResponse(token, "Bearer", _jwtSettings.ExpiresMinutes * 60));
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (!_userStore.ValidateCredentials(request.Email, request.Password))
        {
            return Unauthorized("Invalid credentials.");
        }

        var user = _userStore.FindByEmail(request.Email)!;
        var token = _jwtService.GenerateToken(user.Id, user.Email, TimeSpan.FromMinutes(_jwtSettings.ExpiresMinutes)); 
        return Ok(new LoginResponse(token, "Bearer", _jwtSettings.ExpiresMinutes * 60));
    }

    [HttpPost("validate")]
    public ActionResult Validate([FromBody] string token)
    {
        var result = _jwtService.ValidateToken(token);
        if (!result.IsSuccess)
        {
            return Unauthorized("Invalid token.");
        }

        return Ok(new { valid = true, claims = result.Value!.Claims.Select(c => new { c.Type, c.Value }) });
    }
}
