using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
    
namespace rapidCRUD.ServiceDefaults.Authentication;

public static class KeycloakJwtSetup
{
    public static IServiceCollection AddKeycloakJwtSetup(this IServiceCollection services, IConfiguration config)
    {
        var keyCloakSection = config.GetSection("Keycloak");
        var authority = keyCloakSection.GetValue<string>("Authority");
        var audience = keyCloakSection.GetValue<string>("Audience");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
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