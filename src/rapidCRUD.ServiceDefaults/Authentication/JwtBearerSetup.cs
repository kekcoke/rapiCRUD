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
        config.Bind("Jwt", jwtOptions);
        services.AddSingleton(jwtOptions);

        // Keycloak
        var keycloakOptions = new KeycloakOptions();
        config.Bind("Keycloak", keycloakOptions);
        services.AddSingleton(keycloakOptions);
        
        // Use environment flag to determine which auth to use
        var useKeycloak = config.GetValue<bool>("UseKeycloak", false);
        
        services.AddAuthentication(options =>
            {
                // Use standard "Bearer" scheme name for simplicity
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                if (useKeycloak)
                {
                    // === KEYCLOAK MODE ===
                    Console.WriteLine("[Auth] Using Keycloak authentication");
                    
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
                    
                    // Keycloak will provide its own signing keys via OpenID configuration
                    options.ConfigurationManager = new Microsoft.IdentityModel.Protocols.ConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration>(
                        $"{keycloakOptions.Authority}/.well-known/openid-configuration",
                        new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfigurationRetriever(),
                        new HttpDocumentRetriever { RequireHttps = keycloakOptions.RequireHttpsMetadata }
                    );
                }
                else
                {
                    // === LOCAL JWT MODE (Development/Testing) ===
                    Console.WriteLine("[Auth] Using Local JWT authentication");
                    
                    byte[] keyBytes;
                    try
                    {
                        // Try Base64 first
                        keyBytes = Convert.FromBase64String(jwtOptions.Secret);
                        Console.WriteLine($"[Auth] Secret decoded from Base64: {keyBytes.Length} bytes");
                    }
                    catch (FormatException)
                    {
                        // Fall back to UTF8
                        keyBytes = Encoding.UTF8.GetBytes(jwtOptions.Secret);
                        Console.WriteLine($"[Auth] Secret used as UTF8: {keyBytes.Length} bytes");
                    }
                    
                    if (keyBytes.Length < 32)
                    {
                        throw new InvalidOperationException(
                            $"JWT Secret must be at least 32 bytes (256 bits) for HS256. Current: {keyBytes.Length} bytes. " +
                            $"Please use a Base64-encoded key of at least 32 bytes.");
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
                    
                    Console.WriteLine($"[Auth] JWT Config - Issuer: {jwtOptions.Issuer}, Audience: {jwtOptions.Audience}");
                }
                
                // Add events for debugging
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        if (token != null)
                        {
                            Console.WriteLine($"[Auth] Token received (first 20 chars): {token.Substring(0, Math.Min(20, token.Length))}...");
                        }
                        return System.Threading.Tasks.Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[Auth] ❌ Authentication failed: {context.Exception.GetType().Name}");
                        Console.WriteLine($"[Auth] ❌ Message: {context.Exception.Message}");
                        if (context.Exception.InnerException != null)
                        {
                            Console.WriteLine($"[Auth] ❌ Inner: {context.Exception.InnerException.Message}");
                        }
                        return System.Threading.Tasks.Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var userName = context.Principal?.Identity?.Name ?? "Unknown";
                        var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>();
                        Console.WriteLine($"[Auth] ✅ Token validated for user: {userName}");
                        Console.WriteLine($"[Auth] ✅ Claims: {string.Join(", ", claims)}");
                        return System.Threading.Tasks.Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"[Auth] ⚠️ Challenge issued: {context.Error}, {context.ErrorDescription}");
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireClaim("role", "admin");
            });
            options.AddPolicy("UserOrAdmin", policy =>
            {
                policy.RequireClaim("role", new[] { "user", "admin" });
            });
        });
        
        return services;
    }
}