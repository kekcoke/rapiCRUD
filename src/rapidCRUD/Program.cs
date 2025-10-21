using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using rapidCRUD.Features.Items;
using rapidCRUD.Infrastructure.Database;
using rapidCRUD.Middleware;
using rapidCRUD.ServiceDefaults.Authentication;
using rapidCRUD.ServiceDefaults.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(builder.Configuration["ApplicationInsights:ConnectionString"], TelemetryConverter.Traces)
    .CreateLogger();
    
builder.Host.UseSerilog();

// Services
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new(1, 0);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Database Configuration with Connection Pooling
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "Postgresql";
var connectionString = builder.Configuration[$"ConnectionStrings:{dbProvider}"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (dbProvider.ToLower())
    {
        case "postgresql":
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                npgsqlOptions.CommandTimeout(30);
            });
            break;
        case "sqlserver":
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                sqlOptions.CommandTimeout(30);
            });
            break;
        default:
            throw new InvalidOperationException($"Unsupported database provider: {dbProvider}");
    }
    
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});


// Authentication and Authorization and Service Defaults via ServiceCollectionExtensions
builder.Services
    .AddServiceDefaults(builder.Configuration)
    .AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Rapid CRUD API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"   
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// MassTransit Configuration
var messagingProvider = builder.Configuration["MessagingProvider"] ?? "RabbitMQ";

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // Add consumers
    switch (messagingProvider.ToLower())
    {
        case "azureservicebus":
            var conn = builder.Configuration["AzureServiceBus:ConnectionString"];
            if (!string.IsNullOrEmpty(conn))
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(conn);
                    cfg.ConfigureEndpoints(context);
                });
            }

            break;

        case "rabbitmq":
            var host = builder.Configuration["RabbitMQ:Host"];
            var user = builder.Configuration["RabbitMQ:Username"];
            var password = builder.Configuration["RabbitMQ:Password"];

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(password);
                });
                cfg.ConfigureEndpoints(context);
            });
            break;

        default:
            throw new InvalidOperationException($"Unsupported messaging provider: {messagingProvider}");
    }
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("Database")
    .AddCheck("messaging", () => 
    {
        // Custom messaging health check
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Feature services
builder.Services.AddScoped<IItemRepository, ItemRepository>();

var app = builder.Build();

// Global Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rapid CRUD API v1");
        c.RoutePrefix = "swagger";
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Health Checks Endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions()
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("health/live", new HealthCheckOptions()
{
    Predicate = _ => false
});

// Feature endpoints
app.MapItemEndpoints();

// Run db migrations at startup
if (builder.Configuration.GetValue<bool>("RunMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

Log.Information("Starting rapidCRUD application...");

try
{
    await app.RunAsync();
}

catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
