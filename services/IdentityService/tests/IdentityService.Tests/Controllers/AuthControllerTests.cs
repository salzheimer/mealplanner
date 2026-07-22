using IdentityService.Controllers;
using IdentityService.Contracts;
using IdentityService.Models;
using IdentityService.Interfaces;
using IdentityService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    private readonly Mock<ILookupCache> _lookupCache;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly AuthController _controller;
    private static readonly Guid TestUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public AuthControllerTests()
    {
        _userService = new Mock<IUserService>();
        _sessionRepository = new Mock<ISessionRepository>();
        _lookupCache = new Mock<ILookupCache>();
        _jwtSettings = new JwtSettings("test-issuer", "test-audience", "test-secret-key-must-be-32-chars!!", 60);
        _tokenService = new TokenService(_jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.Secret);

        _sessionRepository.Setup(s => s.CreateAsync(It.IsAny<Session>()))
            .ReturnsAsync((Session s) => s);

        _lookupCache.Setup(c => c.GetClientTypeId(It.IsAny<string>())).Returns(1);

        _controller = new AuthController(
            _userService.Object,
            _sessionRepository.Object,
            _tokenService,
            _jwtSettings,
            _lookupCache.Object,
            new Mock<ILogger<AuthController>>().Object
        );
        SetAuthenticatedUser(_controller, TestUserId);
    }

    private static void SetAuthenticatedUser(ControllerBase controller, Guid userId)
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
            .ReturnsAsync(Result<UserResponse>.Success(new UserResponse(TestUserId, "alice@example.com", "alice")));

        var result = await _controller.Register(new RegisterRequest("alice@example.com", "password123", "alice"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_ValidNewUser_Returns200WithTokens()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponse>.Failure(UserErrors.NotFound));
        _userService.Setup(s => s.ValidatePassword("password123"))
            .ReturnsAsync(Result<string>.Success(""));
        _userService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserRequest>()))
            .ReturnsAsync(Result<UserResponse>.Success(new UserResponse(TestUserId, "alice@example.com", "")));

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
            .ReturnsAsync(Result<UserResponse>.Success(new UserResponse(TestUserId, "alice@example.com", "")));

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
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId, UserId = TestUserId, TokenHash = "hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };

        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(session);
        _sessionRepository.Setup(s => s.RevokeAsync(sessionId)).ReturnsAsync(true);
        _userService.Setup(s => s.FindById(TestUserId))
            .ReturnsAsync(Result<UserResponse>.Success(new UserResponse(TestUserId, "alice@example.com", "")));

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
            Id = Guid.NewGuid(), UserId = TestUserId, TokenHash = "hash",
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
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId, UserId = TestUserId, TokenHash = "hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(session);
        _sessionRepository.Setup(s => s.RevokeAsync(sessionId)).ReturnsAsync(true);

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

    [Fact]
    public async Task Logout_AlreadyRevokedSession_Returns400()
    {
        var session = new Session
        {
            Id = Guid.NewGuid(), UserId = TestUserId, TokenHash = "hash",
            RevokedAt = DateTime.UtcNow.AddMinutes(-5),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(session);

        var result = await _controller.Logout(new RefreshRequest("revoked-token"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Logout_UserIdMismatch_Returns401()
    {
        var otherUserId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var session = new Session
        {
            Id = Guid.NewGuid(), UserId = otherUserId, TokenHash = "hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(session);

        var result = await _controller.Logout(new RefreshRequest("valid-token"));

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    // --- Register: additional failure paths ---

    [Fact]
    public async Task Register_InvalidEmailFormat_Returns400()
    {
        var result = await _controller.Register(new RegisterRequest("not-an-email", "password123", null));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_WeakPassword_Returns500()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponse>.Failure(UserErrors.NotFound));
        _userService.Setup(s => s.ValidatePassword("weak"))
            .ReturnsAsync(Result<string>.Failure(UserErrors.UserPasswordValidationFailed));

        var result = await _controller.Register(new RegisterRequest("alice@example.com", "weak", null));

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task Register_UserCreationFails_Returns400()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponse>.Failure(UserErrors.NotFound));
        _userService.Setup(s => s.ValidatePassword("password123"))
            .ReturnsAsync(Result<string>.Success(""));
        _userService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserRequest>()))
            .ReturnsAsync(Result<UserResponse>.Failure(UserErrors.UpdateFailed));

        var result = await _controller.Register(new RegisterRequest("alice@example.com", "password123", null));

        Assert.IsType<BadRequestResult>(result);
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
        var token = _tokenService.GenerateToken(TestUserId, "alice@example.com", TimeSpan.FromMinutes(60));

        var result = await _controller.Validate(token);

        Assert.IsType<OkObjectResult>(result);
    }

    // --- CheckPassword ---

    [Fact]
    public async Task CheckPassword_ValidPassword_Returns200()
    {
        _userService.Setup(s => s.ValidatePassword("StrongPass1!"))
            .ReturnsAsync(Result<string>.Success(""));

        var result = await _controller.CheckPassword("StrongPass1!");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CheckPassword_WeakPassword_Returns500()
    {
        _userService.Setup(s => s.ValidatePassword("weak"))
            .ReturnsAsync(Result<string>.Failure(UserErrors.UserPasswordValidationFailed));

        var result = await _controller.CheckPassword("weak");

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }
}
