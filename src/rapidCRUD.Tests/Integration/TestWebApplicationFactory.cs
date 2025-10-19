using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace rapidCRUD.Tests.Integration;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug); // FIX: Enable debug logs
        });
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            
            // FIX: Explicitly set UseKeycloak to false for tests
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["UseKeycloak"] = "false",
                ["Jwt:Issuer"] = "rapidCRUD",
                ["Jwt:Audience"] = "rapidCRUD-users",
                ["Jwt:Secret"] = "qykQ/YzwTB/AzmlFikN/43PpNGhvPKd2QoacibuZ974="
            }!);
            
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            // Additional test service configuration if needed
        });
    }
}