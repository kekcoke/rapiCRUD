using System.Net;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using rapidCRUD.Tests.TestHelpers;

namespace rapidCRUD.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly IConfiguration _configuration;

    public AuthIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.Server.PreserveExecutionContext = true;

        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidLocalJwt_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Generate token
        var token = JwtTestHelper.CreateLocalJwtFromConfig(_configuration);
        
        // Debug: Decode and print token details
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        Console.WriteLine("=== TOKEN DETAILS ===");
        Console.WriteLine($"Issuer: {jwtToken.Issuer}");
        Console.WriteLine($"Audiences: {string.Join(", ", jwtToken.Audiences)}");
        Console.WriteLine($"Valid From: {jwtToken.ValidFrom:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Valid To: {jwtToken.ValidTo:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Algorithm: {jwtToken.Header.Alg}");
        Console.WriteLine($"Claims:");
        foreach (var claim in jwtToken.Claims)
        {
            Console.WriteLine($"  {claim.Type}: {claim.Value}");
        }
        Console.WriteLine($"Token (first 50 chars): {token.Substring(0, Math.Min(50, token.Length))}...");
        Console.WriteLine("=====================");
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/test/protected");
        
        // Debug output
        Console.WriteLine($"\n=== RESPONSE ===");
        Console.WriteLine($"Status: {response.StatusCode}");
        Console.WriteLine($"Headers:");
        foreach (var header in response.Headers)
        {
            Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
        }
        
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Content: {content}");
        Console.WriteLine("================\n");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}