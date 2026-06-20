using Shared.Services;
using System.Security.Claims;

using Xunit;

namespace Shared.Tests;

public class TokenServiceTests
{
    private readonly TokenService _TokenService;

    public TokenServiceTests()
    {
        _TokenService = new TokenService("test-issuer", "test-audience", "test-secret-key-must-be-32-chars!!");
    }

    [Fact]
    public void GenerateToken_ValidInputs_ReturnsNonEmptyString()
    {
        var token = _TokenService.GenerateToken(Guid.NewGuid(), "testuser@example.com", TimeSpan.FromMinutes(60));

        Assert.NotEmpty(token);
    }

    [Fact]
    public void ValidateToken_ValidToken_ReturnsSuccess()
    {
        var token = _TokenService.GenerateToken(Guid.NewGuid(), "alice@example.com", TimeSpan.FromMinutes(60));

        var result = _TokenService.ValidateToken(token);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void ValidateToken_ValidToken_ContainsExpectedClaims()
    {
        var userId = Guid.NewGuid();
        var token = _TokenService.GenerateToken(userId, "alice@example.com", TimeSpan.FromMinutes(60));

        var result = _TokenService.ValidateToken(token);

        Assert.True(result.IsSuccess);
        var principal = result.Value!;
        Assert.True(principal.Identity?.IsAuthenticated);
        var claimsIdentity = (ClaimsIdentity)principal.Identity!;
        Assert.Equal(userId.ToString(), claimsIdentity.FindFirst("sub")?.Value);
        Assert.Equal("alice@example.com", claimsIdentity.FindFirst("email")?.Value);
    }

    [Fact]
    public void ValidateToken_InvalidToken_ReturnsFailure()
    {
        var result = _TokenService.ValidateToken("not.a.valid.token");

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void ValidateToken_ExpiredToken_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var token = _TokenService.GenerateToken(userId, "testuser@example.com", TimeSpan.FromMinutes(-2));

        var result = _TokenService.ValidateToken(token);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ValidateToken_TokenFromDifferentSecret_ReturnsFailure()
    {
        var otherService = new TokenService("test-issuer", "test-audience", "different-secret-key-32-chars!!!!");
        var userId = Guid.NewGuid();
        var token = otherService.GenerateToken(userId, "testuser@example.com", TimeSpan.FromMinutes(60));

        var result = _TokenService.ValidateToken(token);

        Assert.False(result.IsSuccess);
    }
}
