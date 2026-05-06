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
using ResourceType = Shared.Models.ResourceType;
using SubjectType = Shared.Models.SubjectType;
using Permission = Shared.Models.Permission;

namespace IdentityService.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IResourcePermissionService> _permissionService;
    private readonly Mock<ISessionRepository> _sessionRepository;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly AuthController _controller;
    private const int UserId = 1;

    public AuthControllerTests()
    {
        _userService = new Mock<IUserService>();
        _permissionService = new Mock<IResourcePermissionService>();
        _sessionRepository = new Mock<ISessionRepository>();
        _jwtSettings = new JwtSettings("test-issuer", "test-audience", "test-secret-key-must-be-32-chars!!", 60);
        _tokenService = new TokenService(_jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.Secret);

        _sessionRepository.Setup(s => s.CreateAsync(It.IsAny<Session>()))
            .ReturnsAsync((Session s) => s);

        _controller = new AuthController(
            _userService.Object,
            _permissionService.Object,
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

    private static ResourcePermissionDto MakePermission(int resourceId = 1) => new(
        1L, ResourceType.Recipe, resourceId, SubjectType.User, 2, Permission.View, UserId
    );

    // --- Register ---

    [Fact]
    public async Task Register_EmptyEmail_ReturnsFailure()
    {
        var request = new RegisterRequest("", "password123", null);

        var result = await _controller.Register(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.MissingEmailOrPassword.Code, result.Error.Code);
    }

    [Fact]
    public async Task Register_EmptyPassword_ReturnsFailure()
    {
        var request = new RegisterRequest("alice@example.com", "", null);

        var result = await _controller.Register(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.MissingEmailOrPassword.Code, result.Error.Code);
    }

    [Fact]
    public async Task Register_UserAlreadyExists_ReturnsFailure()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponseDto?>.Success(new UserResponseDto(1, "alice@example.com", "alice")));

        var result = await _controller.Register(new RegisterRequest("alice@example.com", "password123", "alice"));

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.UserAlreadyExists.Code, result.Error.Code);
    }

    [Fact]
    public async Task Register_ValidNewUser_ReturnsTokens()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponseDto?>.Failure(UserErrors.NotFound));
        _userService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(Result<UserResponseDto>.Success(new UserResponseDto(1, "alice@example.com", "")));

        var result = await _controller.Register(new RegisterRequest("alice@example.com", "password123", null));

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value!.AccessToken);
        Assert.Equal("Bearer", result.Value.TokenType);
    }

    // --- Login ---

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsFailure()
    {
        _userService.Setup(s => s.ValidateCredentials("alice@example.com", "wrongpassword")).ReturnsAsync(false);

        var result = await _controller.Login(new LoginRequest("alice@example.com", "wrongpassword"));

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.Unauthorized.Code, result.Error.Code);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        _userService.Setup(s => s.ValidateCredentials("alice@example.com", "password123")).ReturnsAsync(true);
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponseDto?>.Success(new UserResponseDto(1, "alice@example.com", "")));

        var result = await _controller.Login(new LoginRequest("alice@example.com", "password123"));

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value!.AccessToken);
        Assert.NotEmpty(result.Value.RefreshToken);
    }

    // --- Refresh ---

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokens()
    {
        var refreshToken = "valid-refresh-token";
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

        var result = await _controller.Refresh(new RefreshRequest(refreshToken));

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value!.AccessToken);
    }

    [Fact]
    public async Task Refresh_InvalidToken_ReturnsFailure()
    {
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync((Session?)null);

        var result = await _controller.Refresh(new RefreshRequest("bad-token"));

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidRefreshToken.Code, result.Error.Code);
    }

    [Fact]
    public async Task Refresh_RevokedSession_ReturnsFailure()
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

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidRefreshToken.Code, result.Error.Code);
    }

    // --- Logout ---

    [Fact]
    public async Task Logout_ValidSession_RevokesAndReturnsSuccess()
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

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Logout_SessionNotFound_ReturnsFailure()
    {
        _sessionRepository.Setup(s => s.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync((Session?)null);

        var result = await _controller.Logout(new RefreshRequest("bad-token"));

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidRefreshToken.Code, result.Error.Code);
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

    // --- GrantPermission ---

    [Fact]
    public async Task GrantPermission_ValidRequest_ReturnsPermission()
    {
        var createDto = new ResourcePermissionCreateDto(ResourceType.Recipe, 1, SubjectType.User, 2, Permission.View, UserId, null);
        _permissionService.Setup(s => s.AddPermissionAsync(createDto))
            .ReturnsAsync(Result<ResourcePermissionDto>.Success(MakePermission(1)));

        var result = await _controller.GrantPermission(createDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(ResourceType.Recipe, result.Value!.ResourceType);
    }

    [Fact]
    public async Task GrantPermission_ServiceFailure_ReturnsFailure()
    {
        var createDto = new ResourcePermissionCreateDto(ResourceType.Recipe, 1, SubjectType.User, 2, Permission.View, UserId, null);
        _permissionService.Setup(s => s.AddPermissionAsync(createDto))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(new Error("Permission.UnableToCreate", "Failed to create.")));

        var result = await _controller.GrantPermission(createDto);

        Assert.False(result.IsSuccess);
    }

    // --- GetResourcePermissions ---

    [Fact]
    public async Task GetResourcePermissions_ValidRequest_ReturnsPermissions()
    {
        var permissions = new List<ResourcePermissionDto> { MakePermission(1) };
        _permissionService.Setup(s => s.GetPermissionsForResourceAsync(ResourceType.Recipe, 1))
            .ReturnsAsync(Result<IEnumerable<ResourcePermissionDto>>.Success(permissions));

        var result = await _controller.GetResourcePermissions("Recipe", 1);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task GetResourcePermissions_NoPermissions_ReturnsEmptyList()
    {
        _permissionService.Setup(s => s.GetPermissionsForResourceAsync(ResourceType.Meal, 5))
            .ReturnsAsync(Result<IEnumerable<ResourcePermissionDto>>.Success(new List<ResourcePermissionDto>()));

        var result = await _controller.GetResourcePermissions("Meal", 5);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    // --- GetSubjectPermissions ---

    [Fact]
    public async Task GetSubjectPermissions_ValidRequest_ReturnsPermissions()
    {
        var permissions = new List<ResourcePermissionDto> { MakePermission(1) };
        _permissionService.Setup(s => s.GetPermissionsForSubjectAsync(SubjectType.User, 2))
            .ReturnsAsync(Result<IEnumerable<ResourcePermissionDto>>.Success(permissions));

        var result = await _controller.GetSubjectPermissions("User", 2);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    // --- GetUserPermissions ---

    [Fact]
    public async Task GetUserPermissions_ValidUserId_ReturnsPermissions()
    {
        var permissions = new List<ResourcePermissionDto> { MakePermission(1) };
        _permissionService.Setup(s => s.GetPermissionsForSubjectAsync(SubjectType.User, 2))
            .ReturnsAsync(Result<IEnumerable<ResourcePermissionDto>>.Success(permissions));

        var result = await _controller.GetUserPermissions(2);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    // --- RevokePermission ---

    [Fact]
    public async Task RevokePermission_Granter_ReturnsSuccess()
    {
        var permission = MakePermission(1);
        _permissionService.Setup(s => s.GetPermissionByIdAsync(1L))
            .ReturnsAsync(Result<ResourcePermissionDto?>.Success(permission));
        _permissionService.Setup(s => s.DeletePermissionAsync(1L))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.RevokePermission(1L);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task RevokePermission_PermissionNotFound_ReturnsFailure()
    {
        _permissionService.Setup(s => s.GetPermissionByIdAsync(999L))
            .ReturnsAsync(Result<ResourcePermissionDto?>.Success(null));

        var result = await _controller.RevokePermission(999L);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task RevokePermission_NotGranter_ReturnsUnauthorized()
    {
        var permission = new ResourcePermissionDto(1L, ResourceType.Recipe, 1, SubjectType.User, 2, Permission.View, GrantedBy: 99);
        _permissionService.Setup(s => s.GetPermissionByIdAsync(1L))
            .ReturnsAsync(Result<ResourcePermissionDto?>.Success(permission));

        var result = await _controller.RevokePermission(1L);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.Unauthorized.Code, result.Error.Code);
    }
}
