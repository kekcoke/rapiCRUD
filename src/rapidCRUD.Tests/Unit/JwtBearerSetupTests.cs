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
            {"JwtOptions:Issuer", "local-issuer"},
            {"JwtOptions:Audience", "local-audience"},
            {"JwtOptions:Secret", "super-secret-key"},
            {"KeycloakOptions:Authority", "https://keycloak.local"},
            {"KeycloakOptions:Audience", "keycloak-audience"},
            {"KeycloakOptions:RequireHttpsMetadata", "false"}
        };    
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
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

        Assert.NotNull(keycloakOptions);
        Assert.Equal("https://keycloak.local", keycloakOptions.Authority);

        Assert.NotNull(schemeProvider);
        var scheme = schemeProvider
            .GetSchemeAsync("Bearer").Result;
        
        Assert.NotNull(scheme);
        Assert.Equal("Bearer", scheme.Name);
    }
}