using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace rapidCRUD.Tests.TestHelpers;

public static class JwtTestHelper
{
    /// <summary>
    /// Reads JWT configuration from appsettings and generates a signed token.
    /// </summary>
    public static string CreateLocalJwtFromConfig(IConfiguration configuration, int expiryMinutes = 60)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var secret = configuration["Jwt:Secret"];

        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("Jwt:Secret is missing from configuration.");

        return CreateLocalJwt(issuer!, audience!, secret, expiryMinutes);
    }

    /// <summary>
    /// Creates a local JWT token using the given parameters.
    /// </summary>
    public static string CreateLocalJwt(
        string issuer,
        string audience,
        string secret,
        int expiryMinutes = 60)
    {
        var keyBytes = Convert.FromBase64String(secret);
        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Role, "Tester")
            },
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
