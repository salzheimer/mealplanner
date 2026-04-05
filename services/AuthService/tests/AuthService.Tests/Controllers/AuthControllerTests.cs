using AuthService.Controllers;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Moq;
using Shared.Models;
using Shared.Services;
using Xunit;

namespace AuthService.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<UserStore> _userStore;
    private readonly JwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userStore = new Mock<UserStore>();
        _jwtSettings = new JwtSettings("test-issuer", "test-audience", "test-secret-key-must-be-32-chars!!", 60);
        _jwtService = new JwtService(_jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.Secret);
        _controller = new AuthController(_userStore.Object, _jwtService, _jwtSettings);
    }

    // --- Register ---

    [Fact]
    public void Register_EmptyEmail_ReturnsBadRequest()
    {
        var request = new RegisterRequest("", "password123", null);

        var result = _controller.Register(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public void Register_EmptyPassword_ReturnsBadRequest()
    {
        var request = new RegisterRequest("alice@example.com", "", null);

        var result = _controller.Register(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public void Register_UserAlreadyExists_ReturnsConflict()
    {
        var existingUser = new User(1, "alice@example.com", "alice");
        _userStore.Setup(s => s.FindByEmail("alice@example.com")).Returns(existingUser);

        var request = new RegisterRequest("alice@example.com", "password123", "alice");
        var result = _controller.Register(request);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public void Register_ValidNewUser_ReturnsOkWithToken()
    {
        _userStore.Setup(s => s.FindByEmail("alice@example.com")).Returns((User?)null);
        _userStore.Setup(s => s.AddUser( "alice@example.com","password123",  null))
                  .Returns(new User(2, "alice@example.com",  "hashedpw",null));

        var request = new RegisterRequest("alice@example.com", "password123", null);
        var result = _controller.Register(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.NotEmpty(response.AccessToken);
        Assert.Equal("Bearer", response.TokenType);
    }

    // --- Login ---

    [Fact]
    public void Login_InvalidCredentials_ReturnsUnauthorized()
    {
        _userStore.Setup(s => s.ValidateCredentials("alice@example.com", "wrongpassword")).Returns(false);

        var request = new LoginRequest("alice@example.com", "wrongpassword");
        var result = _controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsOkWithToken()
    {
        _userStore.Setup(s => s.ValidateCredentials("alice@example.com", "password123")).Returns(true);
        _userStore.Setup(s => s.FindByEmail("alice@example.com"))
                  .Returns(new User(2, "alice@example.com", "hashedpw",null));

        var request = new LoginRequest("alice@example.com", "password123");
        var result = _controller.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.NotEmpty(response.AccessToken);
    }

    // --- Validate ---

    [Fact]
    public void Validate_InvalidToken_ReturnsUnauthorized()
    {
        var result = _controller.Validate("not.a.valid.token");

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void Validate_ValidToken_ReturnsOk()
    {
        var token = _jwtService.GenerateToken(1, "alice@example.com", TimeSpan.FromMinutes(60));

        var result = _controller.Validate(token);

        Assert.IsType<OkObjectResult>(result);
    }
}
