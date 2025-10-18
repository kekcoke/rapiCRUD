using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;

namespace rapidCRUD.ServiceDefaults.Authentication;

public static class JwtBearerSetup
{
    public static IServiceCollection AddCombinedJwtKeycloakSetup(this IServiceCollection services, IConfiguration config)
    {
        // Local JWT setup
        var jwtOptions = new JwtOptions();
        config.Bind(nameof(JwtOptions), jwtOptions);
        services.AddSingleton(jwtOptions);

        // Keycloak
        var keycloakOptions = new KeycloakOptions();
        config.Bind(nameof(keycloakOptions), keycloakOptions);
        services.AddSingleton(keycloakOptions);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context => System.Threading.Tasks.Task.CompletedTask,
                    OnTokenValidated = context => System.Threading.Tasks.Task.CompletedTask
                };
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    
                    // Local JWT signing key
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ClockSkew = TimeSpan.Zero // removes 5-minute default clock skew
                };
                
                // Configure Keycloak metadata retrieval for asymmetric signing keys
                options.ConfigurationManager = new Microsoft.IdentityModel.Protocols.ConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration>(
                    $"{keycloakOptions.Authority}/.well-known/openid-configuration",
                    new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever { RequireHttps = keycloakOptions.RequireHttpsMetadata }
                );
            });
        
        return services;
    }
}