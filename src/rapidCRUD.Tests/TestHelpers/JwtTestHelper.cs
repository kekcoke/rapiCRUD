using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace rapidCRUD.Tests.TestHelpers;

public static class JwtTestHelper
{
    public static string CreateLocalJwt(
        string issuer,
        string audience,
        string secretKey,
        int expiryMinutes = 60)
    {
        // If the secret is Base64, decode it; otherwise, use UTF8
        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(secretKey);
        }
        catch (FormatException)
        {
            keyBytes = Encoding.UTF8.GetBytes(secretKey);
        }

        if (keyBytes.Length < 32) // 256 bits
        {
            throw new ArgumentException("The secret key must be at least 32 bytes (256 bits) long.");
        }

        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[]
            {
                new Claim(
                    ClaimTypes.Name, "TestUser")
            },
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}