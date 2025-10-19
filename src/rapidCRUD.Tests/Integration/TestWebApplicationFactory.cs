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
        });

        // either use appsettings or in-memory configuration
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["UseLocalJwt"] = "true",
                ["Jwt:Issuer"] = "rapidCRUD",
                ["Jwt:Audience"] = "rapidCRUD-users",
                ["Jwt:Secret"] = "qykQ/YzwTB/AzmlFikN/43PpNGhvPKd2QoacibuZ974="
            })
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        });
        
        builder.ConfigureServices(services =>
        {
            // use actual service or mock implementations here
        });

        builder.UseSetting("DetailedErrors", "true");
    }


}