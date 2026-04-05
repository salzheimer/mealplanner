using Shared.Services;
using System.Security.Claims;

using Xunit;

namespace Shared.Tests;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _jwtService = new JwtService("test-issuer", "test-audience", "test-secret-key-must-be-32-chars!!");
    }

    [Fact]
    public void GenerateToken_ValidInputs_ReturnsNonEmptyString()
    {
        var token = _jwtService.GenerateToken(1, "testuser@example.com", TimeSpan.FromMinutes(60));

        Assert.NotEmpty(token);
    }

    [Fact]
    public void ValidateToken_ValidToken_ReturnsSuccess()
    {
        var token = _jwtService.GenerateToken(42, "alice@example.com", TimeSpan.FromMinutes(60));

        var result = _jwtService.ValidateToken(token);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void ValidateToken_ValidToken_ContainsExpectedClaims()
    {
        var token = _jwtService.GenerateToken(42, "alice@example.com", TimeSpan.FromMinutes(60));

        var result = _jwtService.ValidateToken(token);

        Assert.True(result.IsSuccess);
        var principal = result.Value!;
        Assert.True(principal.Identity?.IsAuthenticated);
        var claimsIdentity = (ClaimsIdentity)principal.Identity!;
        Assert.Equal("42", claimsIdentity.FindFirst("sub")?.Value);
        Assert.Equal("alice@example.com", claimsIdentity.FindFirst("email")?.Value);
    }

    [Fact]
    public void ValidateToken_InvalidToken_ReturnsFailure()
    {
        var result = _jwtService.ValidateToken("not.a.valid.token");

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void ValidateToken_ExpiredToken_ReturnsFailure()
    {
        var token = _jwtService.GenerateToken(1, "testuser@example.com", TimeSpan.FromMinutes(-2));

        var result = _jwtService.ValidateToken(token);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ValidateToken_TokenFromDifferentSecret_ReturnsFailure()
    {
        var otherService = new JwtService("test-issuer", "test-audience", "different-secret-key-32-chars!!!!");
        var token = otherService.GenerateToken(1, "testuser@example.com", TimeSpan.FromMinutes(60));

        var result = _jwtService.ValidateToken(token);

        Assert.False(result.IsSuccess);
    }
}
