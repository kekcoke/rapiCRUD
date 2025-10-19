using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using rapidCRUD.ServiceDefaults.Authentication;
using Xunit;

namespace rapidCRUD.Tests.Unit;

public class JwtBearerSetupTests
{
    [Fact]
    public void AddCombinedJwtSetup_RegistersOptionsAndAuthentication()
    {
        // Arrange
        var settings = new Dictionary<string, string>
        {
            // FIX: Use "Jwt:" prefix, not "JwtOptions:"
            {"Jwt:Issuer", "local-issuer"},
            {"Jwt:Audience", "local-audience"},
            {"Jwt:Secret", "c3VwZXItc2VjcmV0LWtleS1tdXN0LWJlLTMyYnl0ZXMtbG9uZw=="}, // Base64, 32+ bytes
        
            // FIX: Use "Keycloak:" prefix, not "KeycloakOptions:"
            {"Keycloak:Authority", "https://keycloak.local"},
            {"Keycloak:Audience", "keycloak-audience"},
            {"Keycloak:RequireHttpsMetadata", "false"}
        };    
    
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    
        var services = new ServiceCollection();
    
        // Act
        services.AddCombinedJwtKeycloakSetup(config);
        var provider = services.BuildServiceProvider();
    
        var jwtOptions = provider.GetService<JwtOptions>();
        var keycloakOptions = provider.GetService<KeycloakOptions>();
        var schemeProvider = provider.GetService<IAuthenticationSchemeProvider>();
    
        // Assert
        Assert.NotNull(jwtOptions);
        Assert.Equal("local-issuer", jwtOptions.Issuer);
        Assert.Equal("local-audience", jwtOptions.Audience);

        Assert.NotNull(keycloakOptions);
        Assert.Equal("https://keycloak.local", keycloakOptions.Authority);

        Assert.NotNull(schemeProvider);
        var scheme = schemeProvider.GetSchemeAsync("Bearer").Result;
    
        Assert.NotNull(scheme);
        Assert.Equal("Bearer", scheme.Name);
    }
}