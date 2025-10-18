using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
    
namespace rapidCRUD.ServiceDefaults.Authentication;

public static class KeycloakJwtSetup
{
    public static IServiceCollection AddKeycloakJwtSetup(this IServiceCollection services, IConfiguration config)
    {
        var keyCloakOptions = new KeycloakOptions();
        config.Bind(nameof(keyCloakOptions), keyCloakOptions);
        
        services.AddSingleton(keyCloakOptions);
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keyCloakOptions.Authority;
                options.Audience = keyCloakOptions.Audience;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true
                };
            });

        return services;
    }
}