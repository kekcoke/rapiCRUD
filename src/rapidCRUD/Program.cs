using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using rapidCRUD.Infrastructure.Database;
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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "PostgreSQL";

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

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();
//app.UseHttpsRedirection();
app.Run();

public partial class Program { }
