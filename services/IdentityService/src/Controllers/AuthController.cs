using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Services;
using IdentityService.Services;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using IdentityService.Models;
using IdentityService.Interfaces;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    
    private readonly IUserService _userService;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthController( IUserService userService, TokenService tokenService, JwtSettings jwtSettings)
    {
       
        _userService=userService;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
    }

    [HttpPost("register")]
    public async Task<Result<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<LoginResponse>.Failure(UserErrors.MissingEmailOrPassword);
        }

        if (await _userService.FindByEmail(request.Email) is not null)
        {
            return Result<LoginResponse>.Failure(UserErrors.UserAlreadyExists);
        }
       
        var user = await _userService.CreateUserAsync( new CreateUserDto(Email:request.Email, Password:request.Password, DisplayName:request.DisplayName));
       
        var token =  _tokenService.GenerateToken(user.Value.Id, user.Value.Email, TimeSpan.FromMinutes(_jwtSettings.ExpiresMinutes));

        return Result<LoginResponse>.Success(new LoginResponse(token, "Bearer", _jwtSettings.ExpiresMinutes * 60));
    }

    [HttpPost("login")]
    public async Task<Result<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!await _userService.ValidateCredentials(request.Email, request.Password))
        {
            return Result<LoginResponse>.Failure(UserErrors.Unauthorized);
        }

        var user = await _userService.FindByEmail(request.Email)!;
        var token =  _tokenService.GenerateToken(user.Value.Id, user.Value.Email, TimeSpan.FromMinutes(_jwtSettings.ExpiresMinutes)); 
        return Result<LoginResponse>.Success(new LoginResponse(token, "Bearer", _jwtSettings.ExpiresMinutes * 60));
    }

    [HttpPost("validate")]
    public Task<IActionResult> Validate([FromBody] string token)
    {
        var result = _tokenService.ValidateToken(token);
        if (!result.IsSuccess)
            return Task.FromResult<IActionResult>(Unauthorized(new { error = result.Error.Description }));

        return Task.FromResult<IActionResult>(Ok(new { valid = true }));
    }
}
