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

namespace IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IUserService _userService;
    private readonly ISessionRepository _sessionRepository;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILookupCache _lookupCache;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ISessionRepository sessionRepository, TokenService tokenService, JwtSettings jwtSettings, ILookupCache lookupCache, ILogger<AuthController> logger)
    {
        _userService = userService;
        _sessionRepository = sessionRepository;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
        _lookupCache = lookupCache;
        _logger = logger;
    }

    private int ResolveClientTypeId()
    {
        var name = Request.Headers["X-Client-Type"].FirstOrDefault()?.ToLowerInvariant() ?? "web";
        try { return _lookupCache.GetClientTypeId(name); }
        catch (KeyNotFoundException) { return _lookupCache.GetClientTypeId("web"); }
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return HandleResult(Result<LoginResponse>.Failure(UserErrors.MissingEmailOrPassword));
        }
        if (!new EmailAddressAttribute().IsValid(request.Email))
        {
            return HandleResult(Result<LoginResponse>.Failure(UserErrors.NotAValidEmailAddress));
        }
        if ((await _userService.FindByEmail(request.Email)).IsSuccess)
        {
            return HandleResult(Result<LoginResponse>.Failure(UserErrors.UserAlreadyExists));
        }
        if (!(await _userService.ValidatePassword(request.Password)).IsSuccess)
        {
            return HandleResult(Result<LoginResponse>.Failure(UserErrors.UserPasswordValidationFailed));
        }

        var user = await _userService.CreateUserAsync(new CreateUserRequest(Email: request.Email, Password: request.Password, DisplayName: request.DisplayName));
        return user.IsSuccess ? await IssueTokens(user.Value!.Id, user.Value.Email) : BadRequest();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!await _userService.ValidateCredentials(request.Email, request.Password))
            return HandleResult(Result<LoginResponse>.Failure(UserErrors.Unauthorized));

        var user = await _userService.FindByEmail(request.Email);
        if (!user.IsSuccess || user.Value == null) return HandleResult(Result<LoginResponse>.Failure(UserErrors.NotFound));
        return await IssueTokens(user.Value!.Id, user.Value.Email);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var session = await _sessionRepository.GetByTokenHashAsync(HashToken(request.RefreshToken));

        if (session == null || session.RevokedAt != null || session.ExpiresAt < DateTime.UtcNow)
            return HandleResult(Result<LoginResponse>.Failure(UserErrors.InvalidRefreshToken));

        var user = await _userService.FindById(session.UserId);
        if (!user.IsSuccess || user.Value == null)
            return HandleResult(Result<LoginResponse>.Failure(UserErrors.NotFound));

        await _sessionRepository.RevokeAsync(session.Id);
        return await IssueTokens(user.Value.Id, user.Value.Email);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
    {
        _logger.LogInformation($"Logout request received for token: {request.RefreshToken}");
        var session = await _sessionRepository.GetByTokenHashAsync(HashToken(request.RefreshToken));
        _logger.LogInformation($"Session lookup result: {(session == null ? "null" : $"found session for user {session.UserId}")}");
        if (session == null || session.RevokedAt != null)
            return HandleResult(Result<bool>.Failure(UserErrors.InvalidRefreshToken));

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId) || session.UserId != userId)
            return HandleResult(Result<bool>.Failure(UserErrors.Unauthorized));

        await _sessionRepository.RevokeAsync(session.Id);
        return HandleResult(Result<bool>.Success(true));
    }

    [HttpPost("validate")]
    public Task<IActionResult> Validate([FromBody] string token)
    {
        var result = _tokenService.ValidateToken(token);
        if (!result.IsSuccess)
            return Task.FromResult<IActionResult>(Unauthorized(new { error = result.Error.Description }));

        return Task.FromResult<IActionResult>(Ok(new { valid = true }));
    }
    [HttpPost("check-password")]
    public async Task<IActionResult> CheckPassword([FromBody] string password)
    {

        return HandleResult(await _userService.ValidatePassword(password));

    }

    private async Task<IActionResult> IssueTokens(Guid userId, string email)
    {
        var accessToken = _tokenService.GenerateToken(userId, email, TimeSpan.FromMinutes(_jwtSettings.ExpiresMinutes));
        var refreshToken = TokenService.GenerateRefreshToken();

        var session = new Session
        {
            UserId = userId,
            TokenHash = HashToken(refreshToken),
            ClientTypeId = ResolveClientTypeId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        await _sessionRepository.CreateAsync(session);

        return HandleResult(Result<LoginResponse>.Success(new LoginResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresInSeconds: _jwtSettings.ExpiresMinutes * 60
        )));
    }
}
