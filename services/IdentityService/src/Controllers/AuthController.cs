using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Services;
using IdentityService.Models;
using IdentityService.Interfaces;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ISessionRepository _sessionRepository;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(IUserService userService, ISessionRepository sessionRepository, TokenService tokenService, JwtSettings jwtSettings)
    {
        _userService = userService;
        _sessionRepository = sessionRepository;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    [HttpPost("register")]
    public async Task<Result<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<LoginResponse>.Failure(UserErrors.MissingEmailOrPassword);

        if ((await _userService.FindByEmail(request.Email)).IsSuccess)
            return Result<LoginResponse>.Failure(UserErrors.UserAlreadyExists);

        var user = await _userService.CreateUserAsync(new CreateUserDto(Email: request.Email, Password: request.Password, DisplayName: request.DisplayName));
        return await IssueTokens(user.Value.Id, user.Value.Email);
    }

    [HttpPost("login")]
    public async Task<Result<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!await _userService.ValidateCredentials(request.Email, request.Password))
            return Result<LoginResponse>.Failure(UserErrors.Unauthorized);

        var user = await _userService.FindByEmail(request.Email);
        return await IssueTokens(user.Value!.Id, user.Value.Email);
    }

    [HttpPost("refresh")]
    public async Task<Result<LoginResponse>> Refresh([FromBody] RefreshRequest request)
    {
        var session = await _sessionRepository.GetByTokenHashAsync(HashToken(request.RefreshToken));

        if (session == null || session.RevokedAt != null || session.ExpiresAt < DateTime.UtcNow)
            return Result<LoginResponse>.Failure(UserErrors.InvalidRefreshToken);

        var user = await _userService.FindById(session.UserId);
        if (!user.IsSuccess || user.Value == null)
            return Result<LoginResponse>.Failure(UserErrors.NotFound);

        await _sessionRepository.RevokeAsync(session.Id);
        return await IssueTokens(user.Value.Id, user.Value.Email);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<Result<bool>> Logout([FromBody] RefreshRequest request)
    {
        var session = await _sessionRepository.GetByTokenHashAsync(HashToken(request.RefreshToken));

        if (session == null || session.RevokedAt != null)
            return Result<bool>.Failure(UserErrors.InvalidRefreshToken);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId) || session.UserId != userId)
            return Result<bool>.Failure(UserErrors.Unauthorized);

        await _sessionRepository.RevokeAsync(session.Id);
        return Result<bool>.Success(true);
    }

    [HttpPost("validate")]
    public Task<IActionResult> Validate([FromBody] string token)
    {
        var result = _tokenService.ValidateToken(token);
        if (!result.IsSuccess)
            return Task.FromResult<IActionResult>(Unauthorized(new { error = result.Error.Description }));

        return Task.FromResult<IActionResult>(Ok(new { valid = true }));
    }

    private async Task<Result<LoginResponse>> IssueTokens(int userId, string email)
    {
        var accessToken = _tokenService.GenerateToken(userId, email, TimeSpan.FromMinutes(_jwtSettings.ExpiresMinutes));
        var refreshToken = TokenService.GenerateRefreshToken();

        var session = new Session
        {
            UserId = userId,
            TokenHash = HashToken(refreshToken),
            ClientType = Shared.Models.ClientType.Api,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        await _sessionRepository.CreateAsync(session);

        return Result<LoginResponse>.Success(new LoginResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresInSeconds: _jwtSettings.ExpiresMinutes * 60
        ));
    }
}
