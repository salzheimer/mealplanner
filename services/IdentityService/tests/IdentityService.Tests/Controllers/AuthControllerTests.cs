using IdentityService.Controllers;
using IdentityService.Models;
using IdentityService.Services;
using IdentityService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Models;
using Shared.Services;
using Xunit;

namespace IdentityService.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _userService;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userService = new Mock<IUserService>();
        _jwtSettings = new JwtSettings("test-issuer", "test-audience", "test-secret-key-must-be-32-chars!!", 60);
        _tokenService = new TokenService(_jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.Secret);
        _controller = new AuthController(_userService.Object, _tokenService, _jwtSettings);
    }

    // --- Register ---

    [Fact]
    public async Task Register_EmptyEmail_ReturnsBadRequest()
    {
        var request = new RegisterRequest("", "password123", null);

        var result = await _controller.Register(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.MissingEmailOrPassword.Code, result.Error.Code);
    }

    [Fact]
    public async Task Register_EmptyPassword_ReturnsBadRequest()
    {
        var request = new RegisterRequest("alice@example.com", "", null);

        var result = await _controller.Register(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.MissingEmailOrPassword.Code, result.Error.Code);
    }

    [Fact]
    public async Task Register_UserAlreadyExists_ReturnsConflict()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponseDto?>.Success(new UserResponseDto(1, "alice@example.com", "alice")));

        var request = new RegisterRequest("alice@example.com", "password123", "alice");
        var result = await _controller.Register(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.UserAlreadyExists.Code, result.Error.Code);
    }

    [Fact]
    public async Task Register_ValidNewUser_ReturnsOkWithToken()
    {
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync((Result<UserResponseDto?>)null!);
        _userService.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(Result<UserResponseDto>.Success(new UserResponseDto(1, "alice@example.com", "")));

        var request = new RegisterRequest("alice@example.com", "password123", null);
        var result = await _controller.Register(request);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value!.AccessToken);
        Assert.Equal("Bearer", result.Value.TokenType);
    }

    // --- Login ---

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        _userService.Setup(s => s.ValidateCredentials("alice@example.com", "wrongpassword")).ReturnsAsync(false);

        var request = new LoginRequest("alice@example.com", "wrongpassword");
        var result = await _controller.Login(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.Unauthorized.Code, result.Error.Code);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        _userService.Setup(s => s.ValidateCredentials("alice@example.com", "password123")).ReturnsAsync(true);
        _userService.Setup(s => s.FindByEmail("alice@example.com"))
            .ReturnsAsync(Result<UserResponseDto?>.Success(new UserResponseDto(1, "alice@example.com", "")));

        var request = new LoginRequest("alice@example.com", "password123");
        var result = await _controller.Login(request);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value!.AccessToken);
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
