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
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Override to ensure test settings
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["UseKeycloak"] = "false",
                ["Jwt:Issuer"] = "rapidCRUD",
                ["Jwt:Audience"] = "rapidCRUD-users",
                ["Jwt:Secret"] = "qykQ/YzwTB/AzmlFikN/43PpNGhvPKd2QoacibuZ974="
            }!);
        });

        builder.ConfigureServices(services =>
        {
            // Build a temporary service provider to check configuration
            var sp = services.BuildServiceProvider();
            var config = sp.GetRequiredService<IConfiguration>();
            
            Console.WriteLine("\n=== TEST CONFIGURATION ===");
            Console.WriteLine($"UseKeycloak: {config["UseKeycloak"]}");
            Console.WriteLine($"Jwt:Issuer: {config["Jwt:Issuer"]}");
            Console.WriteLine($"Jwt:Audience: {config["Jwt:Audience"]}");
            Console.WriteLine($"Jwt:Secret (length): {config["Jwt:Secret"]?.Length ?? 0}");
            
            // Check authentication schemes
            var schemeProvider = sp.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            if (schemeProvider != null)
            {
                var schemes = schemeProvider.GetAllSchemesAsync().Result;
                Console.WriteLine($"\n=== AUTH SCHEMES ===");
                foreach (var scheme in schemes)
                {
                    Console.WriteLine($"  {scheme.Name}: {scheme.HandlerType?.Name}");
                }
            }
            Console.WriteLine("======================\n");
        });
    }
}