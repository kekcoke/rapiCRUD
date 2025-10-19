using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
                // Default to local JWT for testing/development
                options.DefaultAuthenticateScheme = "LocalJwt";
                options.DefaultChallengeScheme = "LocalJwt";
            })
            // Local JWT Scheme
            .AddJwtBearer("LocalJwt", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Convert.FromBase64String(jwtOptions.Secret)), // Fixed: Use Base64 decoding
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            })
            // Keycloak Scheme
            .AddJwtBearer("Keycloak", options =>
            {
                options.Authority = keycloakOptions.Authority;
                options.Audience = keycloakOptions.Audience;
                options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = keycloakOptions.Authority,
                    ValidAudience = keycloakOptions.Audience
                };
            });
        
        // Add policy to accept either scheme
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAuthentication", policy =>
            {
                policy.AddAuthenticationSchemes("LocalJwt", "Keycloak");
                policy.RequireAuthenticatedUser();
            });
        });
        
        return services;
    }
}