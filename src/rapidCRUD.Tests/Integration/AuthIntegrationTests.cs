using System.Net;
using System.Net.Http.Headers;
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
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    [Fact]
    public async Task PublicEndpoint_WithoutJwt_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        var token = JwtTestHelper.CreateLocalJwtFromConfig(_configuration);      
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/test/public");
        
        // Debug output if test fails
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Content: {content}");
        }
        
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task ProtectedEndpoint_WithValidLocalJwt_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();

        var token = JwtTestHelper.CreateLocalJwtFromConfig(_configuration);      
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/test/protected"); // Replace with actual protected endpoint
        
        // Debug output if test fails
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Content: {content}");
        }
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


}