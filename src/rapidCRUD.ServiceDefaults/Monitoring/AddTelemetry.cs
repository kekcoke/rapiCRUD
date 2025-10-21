using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace rapidCRUD.ServiceDefaults.Monitoring;

public static class AddTelemetry
{
    public static IServiceCollection ConfigTelemetry(this IServiceCollection services, IConfiguration config)
    {
        var serviceName = config["OpenTelemetry:ServiceName"] ?? "MyApp";
        var endpoint = config["OpenTelemetry:Otlp:Endpoint"];
        var cloudProvider = config["OpenTelemetry:CloudProvider"]?.ToLowerInvariant() ?? "auto";

        var environment = config["OpenTelemetry:Environment"] ?? "production";
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName)
            .AddAttributes(new[]
            {
                new KeyValuePair<string, object>("deployment.environment", environment)
            });

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithTracing(t =>
            {
                t.SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddSource("MassTransit") // ensures MT traces
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(endpoint);
                    });
            })
            .WithMetrics(m =>
            {
                m.SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(endpoint);
                    });
            });

        return services;
    }
}