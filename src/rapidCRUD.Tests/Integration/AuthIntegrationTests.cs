using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using rapidCRUD.Tests.TestHelpers;
using rapidCRUD;

namespace rapidCRUD.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task ProtectedEndpoint_WithValidLocalJwt_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();

        var token = JwtTestHelper.CreateLocalJwt(
            issuer: "local-issuer-1",
            audience: "local-audience-1",
            secretKey: "super-secret-key-1");        
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await client.GetAsync("/api/v1/protected/endpoint"); // Replace with actual protected endpoint
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


}