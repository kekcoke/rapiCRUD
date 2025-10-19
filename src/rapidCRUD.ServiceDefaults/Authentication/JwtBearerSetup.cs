using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;

namespace rapidCRUD.ServiceDefaults.Authentication;

public static class JwtBearerSetup
{
    public static IServiceCollection AddCombinedJwtKeycloakSetup(
        this IServiceCollection services, 
        IConfiguration config)
    {
        // Local JWT setup
        var jwtOptions = new JwtOptions();
        config.Bind("Jwt", jwtOptions);  // FIX: Changed from nameof(JwtOptions)
        services.AddSingleton(jwtOptions);

        // Keycloak
        var keycloakOptions = new KeycloakOptions();
        config.Bind("Keycloak", keycloakOptions);  // FIX: Changed from nameof(keycloakOptions)
        services.AddSingleton(keycloakOptions);
        
        // Determine which auth to use (default to local for development/testing)
        var useKeycloak = config.GetValue<bool>("UseKeycloak", false);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                if (useKeycloak)
                {
                    // KEYCLOAK MODE
                    options.Authority = keycloakOptions.Authority;
                    options.Audience = keycloakOptions.Audience;
                    options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = keycloakOptions.Authority,
                        ValidAudience = keycloakOptions.Audience,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                    
                    // Let the middleware fetch Keycloak's signing keys automatically
                    options.ConfigurationManager = new Microsoft.IdentityModel.Protocols.ConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration>(
                        $"{keycloakOptions.Authority}/.well-known/openid-configuration",
                        new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfigurationRetriever(),
                        new HttpDocumentRetriever { RequireHttps = keycloakOptions.RequireHttpsMetadata }
                    );
                }
                else
                {
                    // LOCAL JWT MODE (for development/testing)
                    byte[] keyBytes;
                    try
                    {
                        // Try Base64 first (recommended for secrets)
                        keyBytes = Convert.FromBase64String(jwtOptions.Secret);
                    }
                    catch (FormatException)
                    {
                        // Fall back to UTF8 if not Base64
                        keyBytes = Encoding.UTF8.GetBytes(jwtOptions.Secret);
                    }
                    
                    if (keyBytes.Length < 32)
                    {
                        throw new InvalidOperationException(
                            "JWT Secret must be at least 32 bytes (256 bits) for HS256 signing. " +
                            $"Current length: {keyBytes.Length} bytes");
                    }
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                    
                    // DO NOT set ConfigurationManager in local mode
                }
                
                // Optional: Add events for debugging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return System.Threading.Tasks.Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"Token validated for: {context.Principal?.Identity?.Name}");
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });
        
        return services;
    }
}