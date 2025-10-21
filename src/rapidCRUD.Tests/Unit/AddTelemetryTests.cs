using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Xunit;
using FluentAssertions;
using rapidCRUD.ServiceDefaults.Monitoring;

namespace rapidCRUD.Tests.Unit;

public class AddTelemetryTests
{
    [Fact]
    public void ConfigTelemtry_ShouldRegisterOpenTelemetryServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = Shared.SharedMethods
            .CreateConfiguration(new Dictionary<string, string>()
            {
                ["OpenTelemetry:ServiceName"] = "TestService",
                ["OpenTelemetry:Otlp:Endpoint"] = "http://localhost:4317",
                ["OpenTelemetry:Environment"] = "test"
            });

        // Act
        services.ConfigTelemetry(config);
        
        // Assert
        var traceProvider = services.BuildServiceProvider()
            .GetService<TracerProvider>();
        traceProvider.Should().NotBeNull();
        
        var meterProvider = services.BuildServiceProvider()
            .GetService<MeterProvider>();
        meterProvider.Should().NotBeNull();
    }

    [Fact]
    public void ConfigTelemtry_WithMissingServiceName_ShouldUseDefaultName()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = Shared.SharedMethods
            .CreateConfiguration(new Dictionary<string, string>()
            {
                // ServiceName is missing
                ["OpenTelemetry:Otlp:Endpoint"] = "http://localhost:4317",
                ["OpenTelemetry:Environment"] = "test"
            });
        
        // Act
        services.ConfigTelemetry(config);
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        var traceProvider = serviceProvider
            .GetService<TracerProvider>();
        traceProvider.Should().NotBeNull();
    }
    
    [Fact]
    public void ConfigTelemetry_WithMissingEnvironment_ShouldUseProductionDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Shared.SharedMethods.CreateConfiguration(new Dictionary<string, string>
        {
            ["OpenTelemetry:ServiceName"] = "TestService",
            ["OpenTelemetry:Otlp:Endpoint"] = "http://localhost:4317"
        });

        // Act
        services.ConfigTelemetry(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert 
        var tracerProvider = serviceProvider.GetService<TracerProvider>();
        tracerProvider.Should().NotBeNull();
    }

    [Theory]
    [InlineData("aws")]
    [InlineData("azure")]
    [InlineData("Development")]
    public void ConfigTelemetry_WithDifferentEnvironments_ShouldConfigureCorrectly(string env)
    {
        // Arrange
        var services = new ServiceCollection();
        var config = Shared.SharedMethods.CreateConfiguration(new Dictionary<string, string>()
        {
            ["OpenTelemetry:ServiceName"] = "TestService",
            ["OpenTelemetry:Otlp:Endpoint"] = "http://localhost:4317",
            ["OpenTelemetry:Environment"] = env
        });

        // Act
        services.ConfigTelemetry(config);
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        var tracerProvider = serviceProvider.GetService<TracerProvider>();
        tracerProvider.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData("http://localhost:4317")]
    [InlineData("http://otel-collector:4317")]
    [InlineData("https://api.honeycomb.io:443")]
    public void ConfigTelemetry_WithDifferentEndpoints_ShouldConfigureCorrectly(string endpoint)
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Shared.SharedMethods.CreateConfiguration(new Dictionary<string, string>
        {
            ["OpenTelemetry:ServiceName"] = "TestService",
            ["OpenTelemetry:Otlp:Endpoint"] = endpoint,
            ["OpenTelemetry:Environment"] = "test"
        });

        // Act
        services.ConfigTelemetry(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tracerProvider = serviceProvider.GetService<TracerProvider>();
        tracerProvider.Should().NotBeNull();
    }
    
    [Fact]
    public void ConfigTelemetry_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Shared.SharedMethods.CreateConfiguration(new Dictionary<string, string>
        {
            ["OpenTelemetry:ServiceName"] = "TestService",
            ["OpenTelemetry:Otlp:Endpoint"] = "http://localhost:4317"
        });

        // Act
        var result = services.ConfigTelemetry(configuration);

        // Assert
        result.Should().BeSameAs(services, "method should return the same IServiceCollection for chaining");
    }
    [Fact]
    public void ConfigTelemetry_WithNullEndpoint_ShouldThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Shared.SharedMethods.CreateConfiguration(new Dictionary<string, string>
        {
            ["OpenTelemetry:ServiceName"] = "TestService"
        });

        // Act
        var act = () =>
        {
            services.ConfigTelemetry(configuration);
            services.BuildServiceProvider();
        };

        // Assert
        act.Should().Throw<Exception>("endpoint is required for OTLP exporter");
    }

    [Theory]
    [InlineData("aws")]
    [InlineData("azure")]
    [InlineData("gcp")]
    [InlineData("auto")]
    public void ConfigTelemetry_WithDifferentCloudProviders_ShouldConfigureCorrectly(string cloudProvider)
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Shared.SharedMethods.CreateConfiguration(new Dictionary<string, string>
        {
            ["OpenTelemetry:ServiceName"] = "TestService",
            ["OpenTelemetry:Otlp:Endpoint"] = "http://localhost:4317",
            ["OpenTelemetry:CloudProvider"] = cloudProvider
        });

        // Act
        services.ConfigTelemetry(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tracerProvider = serviceProvider.GetService<TracerProvider>();
        tracerProvider.Should().NotBeNull();
    }
}