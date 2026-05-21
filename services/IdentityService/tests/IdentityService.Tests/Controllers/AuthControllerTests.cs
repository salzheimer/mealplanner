using IdentityService.Controllers;
using IdentityService.Models;
using IdentityService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Models;
using Shared.Services;
using System.Security.Claims;
using Xunit;

namespace IdentityService.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<ISessionRepository> _sessionRepository;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly AuthController _controller;
    private const int UserId = 1;

    public AuthControllerTests()
    {
        _userService = new Mock<IUserService>();
        _sessionRepository = new Mock<ISessionRepository>();
        _jwtSettings = new JwtSettings("test-issuer", "test-audience", "test-secret-key-must-be-32-chars!!", 60);
        _tokenService = new TokenService(_jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.Secret);

        _sessionRepository.Setup(s => s.CreateAsync(It.IsAny<Session>()))
            .ReturnsAsync((Session s) => s);

        _controller = new AuthController(
            _userService.Object,
            _sessionRepository.Object,
            _tokenService,
            _jwtSettings
        );
        SetAuthenticatedUser(_controller, UserId);
    }

    private static void SetAuthenticatedUser(ControllerBase controller, int userId)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
            }
        };
    }

    // --- Register ---

    [Fact]
    public async Task Register_EmptyEmail_Returns400()
    {
        var result = await _controller.Register(new RegisterRequest("", "password123", null));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_EmptyPassword_Returns400()
    {
        var result = await _controller.Register(new RegisterRequest("alice@example.com", "", null));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_UserAlreadyExists_Returns400()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponseDto?>.Success(new UserResponseDto(1, "alice@example.com", "alice")));

        var result = await _controller.Register(new RegisterRequest("alice@example.com", "password123", "alice"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_ValidNewUser_Returns200WithTokens()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponseDto?>.Failure(UserErrors.NotFound));
        _userService.Setup(s => s.ValidatePassword("password123"))
            .ReturnsAsync(Result<string>.Success(""));
        _userService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(Result<UserResponseDto>.Success(new UserResponseDto(1, "alice@example.com", "")));

        var result = await _controller.Register(new RegisterRequest("alice@example.com", "password123", null));

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<LoginResponse>(ok.Value);
        Assert.NotEmpty(value.AccessToken);
    }

    // --- Login ---

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        _userService.Setup(s => s.ValidateCredentials("alice@example.com", "wrongpassword")).ReturnsAsync(false);

        var result = await _controller.Login(new LoginRequest("alice@example.com", "wrongpassword"));

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithTokens()
    {
        _userService.Setup(s => s.ValidateCredentials("alice@example.com", "password123")).ReturnsAsync(true);
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponseDto?>.Success(new UserResponseDto(1, "alice@example.com", "")));

        var result = await _controller.Login(new LoginRequest("alice@example.com", "password123"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<LoginResponse>(ok.Value);
        Assert.NotEmpty(value.AccessToken);
        Assert.NotEmpty(value.RefreshToken);
    }

    // --- Refresh ---

    [Fact]
    public async Task Refresh_ValidToken_Returns200WithNewTokens()
    {
        var session = new Session
        {
            Id = 1L, UserId = 1, TokenHash = "hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };

        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(session);
        _sessionRepository.Setup(s => s.RevokeAsync(session.Id)).ReturnsAsync(true);
        _userService.Setup(s => s.FindById(1))
            .ReturnsAsync(Result<UserResponseDto?>.Success(new UserResponseDto(1, "alice@example.com", "")));

        var result = await _controller.Refresh(new RefreshRequest("valid-refresh-token"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<LoginResponse>(ok.Value);
        Assert.NotEmpty(value.AccessToken);
    }

    [Fact]
    public async Task Refresh_InvalidToken_Returns400()
    {
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync((Session?)null);

        var result = await _controller.Refresh(new RefreshRequest("bad-token"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Refresh_RevokedSession_Returns400()
    {
        var session = new Session
        {
            Id = 1L, UserId = 1, TokenHash = "hash",
            RevokedAt = DateTime.UtcNow.AddMinutes(-5),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(session);

        var result = await _controller.Refresh(new RefreshRequest("revoked-token"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- Logout ---

    [Fact]
    public async Task Logout_ValidSession_Returns200()
    {
        var session = new Session
        {
            Id = 1L, UserId = UserId, TokenHash = "hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(session);
        _sessionRepository.Setup(s => s.RevokeAsync(session.Id)).ReturnsAsync(true);

        var result = await _controller.Logout(new RefreshRequest("valid-token"));

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task Logout_SessionNotFound_Returns400()
    {
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync((Session?)null);

        var result = await _controller.Logout(new RefreshRequest("bad-token"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- Validate ---

    [Fact]
    public async Task Validate_InvalidToken_ReturnsUnauthorized()
    {
        var result = await _controller.Validate("not.a.valid.token");

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Validate_ValidToken_ReturnsOk()
    {
        var token = _tokenService.GenerateToken(1, "alice@example.com", TimeSpan.FromMinutes(60));

        var result = await _controller.Validate(token);

        Assert.IsType<OkObjectResult>(result);
    }
}
