using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Result = Shared.Models.Result<System.Security.Claims.ClaimsPrincipal>;

namespace Shared.Services;

public class JwtService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secret;

    public JwtService(string issuer, string audience, string secret)
    {
        _issuer = issuer;
        _audience = audience;
        _secret = secret;
    }

    public string GenerateToken(int userId, string email, TimeSpan expiresIn)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(expiresIn),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Result ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var key = Encoding.UTF8.GetBytes(_secret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return Result.Success(principal);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
