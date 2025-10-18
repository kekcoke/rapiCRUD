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
        // Try to interpret the key as Base64, otherwise as plain text
        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(secret);
        }
        catch (FormatException)
        {
            keyBytes = Encoding.UTF8.GetBytes(secret);
        }

        if (keyBytes.Length < 32) // 256 bits required
            throw new ArgumentException("The secret key must be at least 32 bytes (256 bits) long for HS256.", nameof(secret));

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
