using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            logging.SetMinimumLevel(LogLevel.Debug);
        });
        
        // DON'T clear config sources - let it load normally first
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add test-specific overrides AFTER existing config
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["UseKeycloak"] = "false",
                ["Jwt:Issuer"] = "rapidCRUD",
                ["Jwt:Audience"] = "rapidCRUD-users",
                ["Jwt:Secret"] = "qykQ/YzwTB/AzmlFikN/43PpNGhvPKd2QoacibuZ974=",
                ["Jwt:ExpirationMinutes"] = "60"
            }!);
        });

        builder.ConfigureServices(services =>
        {
            // Debug: Print registered authentication schemes
            var serviceProvider = services.BuildServiceProvider();
            var schemeProvider = serviceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            
            if (schemeProvider != null)
            {
                var schemes = schemeProvider.GetAllSchemesAsync().Result;
                Console.WriteLine("=== Registered Auth Schemes in Test ===");
                foreach (var scheme in schemes)
                {
                    Console.WriteLine($"  Scheme: {scheme.Name}, Handler: {scheme.HandlerType?.Name}");
                }
                Console.WriteLine("========================================");
            }
        });
    }
}