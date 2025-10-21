using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using rapidCRUD.ServiceDefaults.Authentication;
using rapidCRUD.ServiceDefaults.Monitoring;

namespace rapidCRUD.ServiceDefaults.Configuration;

public static class ServiceDefaultsExtensions
{
    public static IServiceCollection AddServiceDefaults(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCombinedJwtKeycloakSetup(configuration)
            .ConfigTelemetry(configuration);
        
        return services;
    }
}