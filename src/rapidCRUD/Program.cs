using Microsoft.OpenApi.Models;
using rapidCRUD.ServiceDefaults.Authentication;
using rapidCRUD.ServiceDefaults.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services
    .AddServiceDefaults(builder.Configuration)
    .AddKeycloakJwtSetup(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddKeycloakJwtSetup(builder.Configuration);
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

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
//app.UseHttpsRedirection();