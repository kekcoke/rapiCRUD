# rapiCRUD - Production-ready .NET 8 API with vertical slice/event architecture, integrated with database, keycloak, AWS &amp; Azure deployment and CI/CD actions.

## 🚀 Quick Start

```bash
# Clone and initialize
git clone 
cd ProductionTemplate

# Option 1: One-command setup (recommended)
make setup

# Option 2: Manual setup
chmod +x scripts/*.sh
./scripts/setup-local.sh

# Start the API
cd src/rapidCRUD
dotnet run

# API available at: https://localhost:7001
# Swagger UI: https://localhost:7001/swagger
# Health checks: https://localhost:7001/health
```


✅ **Production-Ready .NET 8 API**
- Minimal API with top-level statements
- Hot reload and fast startup
- Optimized for performance

✅ **Vertical Slice Architecture**
- Feature-based organization
- Self-contained slices
- Clear separation of concerns

✅ **Database Integration**
- PostgreSQL (Primary)
- SQL Server (Alternative)
- Entity Framework Core 8
- Connection pooling configured
- Automatic migrations (optional)

✅ **Authentication & Authorization**
- Keycloak integration
- JWT Bearer tokens
- OAuth2 flows (Authorization Code, Client Credentials)
- Role-based access control
- Policy-based authorization

✅ **Message Brokers**
- Azure Service Bus (Primary)
- RabbitMQ with MassTransit (Fallback)
- Event-driven architecture
- Consumer patterns
- Local emulator support

✅ **Caching**
- Redis with connection pooling
- Distributed caching
- Local cache options

✅ **Health Checks**
- Database connectivity
- Redis availability
- Message broker status
- Liveness probes
- Readiness probes

✅ **Monitoring & Observability**
- Application Insights integration
- DataDog APM support
- Structured logging with Serilog
- OpenTelemetry tracing
- Distributed tracing

✅ **Cloud Deployments**
- **Azure**: Container Apps, PostgreSQL Flexible Server, Service Bus
- **AWS**: ECS Fargate, RDS PostgreSQL, ElastiCache Redis
- Managed identities
- Infrastructure as Code (Terraform)

✅ **CI/CD Pipelines**
- GitHub Actions workflow
- Azure DevOps pipeline
- Automated testing
- Security scanning
- Automated deployments

✅ **Testing**
- Unit tests with xUnit
- Integration tests with Testcontainers
- Authentication test scenarios
- Messaging mocks
- Code coverage reports

✅ **.NET Aspire**
- Service orchestration
- Service discovery
- Distributed tracing
- Resource management
- Dashboard monitoring

✅ **Docker Support**
- Multi-stage Dockerfile
- Docker Compose for local dev
- Optimized Linux images
- Health check support
- Non-root user

## 📁 Project Structure
```
rapidCRUD/
├── src/
│   ├── rapidCRUD/              # Main API project
│   │   ├── Features/               # Vertical slices
│   │   │   ├── Items/              # Item feature
│   │   │   │   ├── CreateItem.cs
│   │   │   │   ├── GetItem.cs
│   │   │   │   ├── UpdateItem.cs
│   │   │   │   ├── DeleteItem.cs
│   │   │   │   ├── Item.cs
│   │   │   │   ├── ItemEndpoints.cs
│   │   │   │   ├── ItemRepository.cs
│   │   │   │   └── Events.cs
│   │   │   └── Users/              # User feature
│   │   ├── Infrastructure/
│   │   │   ├── Database/
│   │   │   │   └── ApplicationDbContext.cs
│   │   │   ├── Messaging/
│   │   │   ├── Authentication/
│   │   │   └── Monitoring/
│   │   ├── Middleware/
│   │   │   └── GlobalExceptionMiddleware.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── rapidCRUD.csproj
│   ├── rapidCRUD.Contracts/    # Shared contracts
│   ├── rapidCRUD.AppHost/      # Aspire orchestration
│   ├── rapidCRUD.ServiceDefaults/ # Shared defaults
│   └── rapidCRUD.Tests/        # Test project
│       ├── Unit/
│       ├── Integration/
│       └── E2E/
├── infrastructure/
│   ├── terraform/
│   │   ├── azure/
│   │   │   ├── main.tf
│   │   │   ├── variables.tf
│   │   │   └── outputs.tf
│   │   └── aws/
│   │       ├── main.tf
│   │       ├── variables.tf
│   │       └── outputs.tf
│   └── docker/
├── scripts/
│   ├── deploy-azure.sh
│   ├── deploy-aws.sh
│   ├── setup-local.sh
│   ├── run-tests.sh
│   └── clean.sh
├── .github/workflows/
│   └── ci-cd.yml
├── azure-pipelines.yml
├── docker-compose.yml
├── Dockerfile
├── Makefile
└── README.md
```

## 🏗️ Architecture

### Vertical Slice Architecture

Each feature is self-contained with:
- Request/Response models
- Validation (FluentValidation)
- Business logic
- Database access
- Event publishing
- Endpoints

**Benefits:**
- High cohesion, low coupling
- Easy to understand and maintain
- Simple to add new features
- Testable in isolation

### Technology Stack

| Component | Technology | Purpose |
|-----------|-----------|---------|
| Runtime | .NET 8 | Latest LTS framework |
| Database | PostgreSQL 16 | Primary data store |
| Cache | Redis 7 | Distributed caching |
| Identity | Keycloak 23 | Authentication & SSO |
| Messaging | Azure Service Bus / RabbitMQ | Event-driven communication |
| Logging | Serilog | Structured logging |
| Monitoring | App Insights / DataDog | APM and monitoring |
| Testing | xUnit + Testcontainers | Comprehensive testing |
| CI/CD | GitHub Actions / Azure DevOps | Automated pipelines |
| IaC | Terraform | Infrastructure automation |

## 🔐 Authentication & Authorization

### Keycloak Configuration

1. **Access Keycloak Admin Console**:
    - URL: http://localhost:8080
    - Username: `admin`
    - Password: `admin`

2. **Create Realm**: `production`

3. **Create Client**:
    - Client ID: `production-api`
    - Client Protocol: `openid-connect`
    - Access Type: `confidential`
    - Valid Redirect URIs: `http://localhost:5000/*`

4. **Create Roles**:
    - `user` - Standard user access
    - `admin` - Administrative access

### Getting Access Tokens

```bash
# Client Credentials Flow
curl -X POST http://localhost:8080/realms/production/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=production-api" \
  -d "client_secret=YOUR_CLIENT_SECRET" \
  -d "grant_type=client_credentials"

# Password Flow (for testing)
curl -X POST http://localhost:8080/realms/production/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=production-api" \
  -d "client_secret=YOUR_CLIENT_SECRET" \
  -d "grant_type=password" \
  -d "username=testuser" \
  -d "password=password"
```

### Using Tokens

```bash
# Store token
TOKEN="eyJhbGc..."

# Call protected endpoint
curl -H "Authorization: Bearer $TOKEN" \
  http://localhost:5000/api/items
```

## 💾 Database Configuration

### PostgreSQL (Recommended)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=productiondb;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=5;MaxPoolSize=100"
  },
  "DatabaseProvider": "PostgreSQL"
}
```

### SQL Server (Alternative)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=productiondb;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "DatabaseProvider": "SqlServer"
}
```

### Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Generate SQL script
dotnet ef migrations script
```

### Connection Pooling

**PostgreSQL Configuration:**
- MinPoolSize: 5
- MaxPoolSize: 100
- Connection Lifetime: 300s
- Command Timeout: 30s

**Best Practices:**
- Use async/await for all database operations
- Dispose DbContext properly (using statement)
- Enable connection pooling
- Monitor connection pool metrics

## 📨 Messaging

### Azure Service Bus (Production)

```json
{
  "MessagingProvider": "AzureServiceBus",
  "AzureServiceBus": {
    "ConnectionString": "Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=..."
  }
}
```

### RabbitMQ (Development/Fallback)

```json
{
  "MessagingProvider": "RabbitMQ",
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Publishing Events

```csharp
// In your endpoint/service
await publishEndpoint.Publish(new ItemCreatedEvent
{
    ItemId = item.Id,
    Name = item.Name,
    CreatedAt = DateTime.UtcNow
});
```

### Consuming Events

```csharp
public class ItemCreatedConsumer : IConsumer
{
    private readonly ILogger _logger;

    public ItemCreatedConsumer(ILogger logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext context)
    {
        _logger.LogInformation(
            "Item created: {ItemId} - {Name}",
            context.Message.ItemId,
            context.Message.Name);

        // Process event
        return Task.CompletedTask;
    }
}
```

## 🏥 Health Checks

### Endpoints

- `/health` - Overall system health (200 OK if healthy)
- `/health/ready` - Readiness probe (K8s compatible)
- `/health/live` - Liveness probe (K8s compatible)

### Monitored Components

- **Database**: Connection and query execution
- **Redis**: Connectivity and ping
- **Message Bus**: Connection status
- **Self**: Application responsiveness

### Custom Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("custom_check", () => 
    {
        // Your custom logic
        bool isHealthy = CheckCustomCondition();
        
        return isHealthy 
            ? HealthCheckResult.Healthy("Service is running")
            : HealthCheckResult.Unhealthy("Service is down");
    });
```

## 📊 Monitoring & Observability

### Application Insights

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=...;IngestionEndpoint=https://..."
  }
}
```

**Available Metrics:**
- Request rates and response times
- Dependency tracking (DB, HTTP, Redis)
- Exception tracking
- Custom events and metrics
- Live metrics stream

### DataDog

```json
{
  "DataDog": {
    "ApiKey": "your-api-key",
    "ServiceName": "production-api",
    "Environment": "production"
  }
}
```

**Features:**
- APM traces
- Custom metrics
- Log aggregation
- Alerting

### Structured Logging

```csharp
// Log with context
_logger.LogInformation(
    "Item {ItemId} created by user {UserId}",
    itemId,
    userId);

// Log with exception
_logger.LogError(ex, 
    "Failed to create item {ItemId}", 
    itemId);
```

## 🐳 Docker Deployment

### Build Image

```bash
# Build
docker build -t production-api:latest .

# Build with specific tag
docker build -t production-api:v1.0.0 .

# Build for specific platform
docker build --platform linux/amd64 -t production-api:latest .
```

### Run Container

```bash
# Basic run
docker run -p 8080:8080 production-api:latest

# With environment variables
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="..." \
  production-api:latest

# With volume mount for logs
docker run -p 8080:8080 \
  -v $(pwd)/logs:/app/logs \
  production-api:latest
```

### Docker Compose

```bash
# Start all services
docker-compose up -d

# Start with specific profile
docker-compose --profile monitoring up -d

# View logs
docker-compose logs -f api

# Stop all services
docker-compose down

# Clean up volumes
docker-compose down -v
```

## ☁️ Cloud Deployment

### Azure (Recommended)

#### Prerequisites
- Azure CLI installed
- Azure subscription
- Terraform installed

#### One-Command Deployment

```bash
# Make script executable
chmod +x scripts/deploy-azure.sh

# Deploy everything
./scripts/deploy-azure.sh

# Or use Makefile
make deploy-azure
```

#### Manual Deployment

```bash
# 1. Deploy infrastructure
cd infrastructure/terraform/azure
terraform init
terraform apply

# 2. Get registry info
REGISTRY=$(terraform output -raw container_registry_login_server)

# 3. Build and push image
az acr login --name $(echo $REGISTRY | cut -d'.' -f1)
docker build -t production-api:latest .
docker tag production-api:latest $REGISTRY/production-api:latest
docker push $REGISTRY/production-api:latest

# 4. Update container app
az containerapp update \
  --name ca-production-api-production \
  --resource-group rg-production-api-production \
  --image $REGISTRY/production-api:latest
```

#### Azure Resources Created

- Resource Group
- Container Apps Environment
- Container App (API)
- PostgreSQL Flexible Server
- Azure Cache for Redis
- Service Bus Namespace
- Container Registry
- Application Insights
- Log Analytics Workspace
- Managed Identity
- Container Instance (Keycloak)

### AWS

#### Prerequisites
- AWS CLI installed
- AWS credentials configured
- Terraform installed

#### One-Command Deployment

```bash
# Make script executable
chmod +x scripts/deploy-aws.sh

# Deploy everything
./scripts/deploy-aws.sh

# Or use Makefile
make deploy-aws
```

#### Manual Deployment

```bash
# 1. Deploy infrastructure
cd infrastructure/terraform/aws
terraform init
terraform apply

# 2. Get ECR repository URL
ECR_URL=$(terraform output -raw ecr_repository_url)

# 3. Login to ECR
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin $ECR_URL

# 4. Build and push
docker build -t production-api:latest .
docker tag production-api:latest $ECR_URL:latest
docker push $ECR_URL:latest

# 5. Update ECS service
aws ecs update-service \
  --cluster production-api-production-cluster \
  --service production-api-production-service \
  --force-new-deployment \
  --region us-east-1
```

#### AWS Resources Created

- VPC with public/private subnets
- Internet Gateway & NAT Gateway
- Application Load Balancer
- ECS Fargate Cluster & Service
- ECR Repository
- RDS PostgreSQL
- ElastiCache Redis
- CloudWatch Log Groups
- IAM Roles & Policies
- Security Groups

## 🧪 Testing

### Run All Tests

```bash
# Using script
./scripts/run-tests.sh

# Using Makefile
make test

# Using dotnet CLI
dotnet test
```

### Run Specific Tests

```bash
# Unit tests only
dotnet test --filter "Category=Unit"

# Integration tests only
dotnet test --filter "Category=Integration"

# Specific test class
dotnet test --filter "FullyQualifiedName~ItemRepositoryTests"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

**Unit Tests** - Fast, isolated, no external dependencies
```csharp
[Trait("Category", "Unit")]
public class ItemRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddItemToDatabase()
    {
        // Arrange, Act, Assert
    }
}
```

**Integration Tests** - Real dependencies with Testcontainers
```csharp
[Trait("Category", "Integration")]
[Collection("Database")]
public class DatabaseTests : IAsyncLifetime
{
    private PostgreSqlContainer _dbContainer;
    
    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder().Build();
        await _dbContainer.StartAsync();
    }
}
```

### Coverage Reports

```bash
# Generate HTML report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:"Html"

# Open report
open coverage-report/index.html
```

## 🔄 CI/CD

### GitHub Actions

**Triggers:**
- Push to `main` or `develop`
- Pull requests
- Manual workflow dispatch

**Pipeline Stages:**
1. **Build & Test** - Compile and run tests
2. **Code Quality** - Static analysis
3. **Security Scan** - Vulnerability scanning
4. **Build Docker** - Create container image
5. **Deploy** - Deploy to cloud
6. **Smoke Tests** - Verify deployment

**Required Secrets:**
```
AZURE_CREDENTIALS
AZURE_RESOURCE_GROUP
AZURE_CONTAINER_APP_NAME
KEYCLOAK_AUTHORITY
AWS_ACCESS_KEY_ID
AWS_SECRET_ACCESS_KEY
SLACK_WEBHOOK_URL
```

### Azure DevOps

**Configuration:**
1. Create Variable Group: `production-variables`
2. Add service connections for Azure
3. Configure environments (dev, staging, production)

**Pipeline Stages:**
- Build
- Security
- BuildDocker
- DeployDev (develop branch)
- DeployProduction (main branch)
- SmokeTests

## 🌐 .NET Aspire

### Run with Aspire

```bash
# Start Aspire AppHost
cd src/rapidCRUD.AppHost
dotnet run

# Aspire Dashboard available at: http://localhost:15888
```

### Features

- **Service Discovery** - Automatic service registration
- **Distributed Tracing** - OpenTelemetry integration
- **Metrics Dashboard** - Real-time metrics visualization
- **Resource Management** - Unified resource configuration
- **Health Monitoring** - Centralized health checks

### Dashboard Sections

- **Resources** - View all services and dependencies
- **Traces** - Distributed request tracing
- **Metrics** - Performance metrics and charts
- **Logs** - Aggregated structured logs
- **Console** - Service output streams

## ⚙️ Configuration

### Environment-Specific Settings

**Development** (appsettings.Development.json):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "RunMigrationsOnStartup": true,
  "MessagingProvider": "RabbitMQ"
}
```

**Production** (appsettings.Production.json):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "RunMigrationsOnStartup": false,
  "MessagingProvider": "AzureServiceBus"
}
```

### Environment Variables

```bash
# Database
DATABASE_CONNECTION_STRING="Host=...;Database=..."
DATABASE_PROVIDER="PostgreSQL"

# Authentication
KEYCLOAK_AUTHORITY="https://keycloak.example.com/realms/production"
KEYCLOAK_CLIENT_ID="production-api"
KEYCLOAK_CLIENT_SECRET="secret"

# Messaging
AZURE_SERVICE_BUS_CONNECTION_STRING="Endpoint=..."
RABBITMQ_HOST="localhost"

# Monitoring
APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=..."
DATADOG_API_KEY="api-key"

# Features
RUN_MIGRATIONS_ON_STARTUP="false"
```

### User Secrets (Development)

```bash
# Initialize user secrets
dotnet user-secrets init

# Set secrets
dotnet user-secrets set "Keycloak:ClientSecret" "your-secret"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection"

# List secrets
dotnet user-secrets list
```

## 🚦 API Usage Examples

### Create Item

```bash
curl -X POST https://api.example.com/api/items \
  -H "Authorization: Bearer ${TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Sample Item",
    "description": "This is a sample item"
  }'
```

### Get Item

```bash
curl https://api.example.com/api/items/123e4567-e89b-12d3-a456-426614174000 \
  -H "Authorization: Bearer ${TOKEN}"
```

### Update Item

```bash
curl -X PUT https://api.example.com/api/items/123e4567-e89b-12d3-a456-426614174000 \
  -H "Authorization: Bearer ${TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Updated Item",
    "description": "Updated description"
  }'
```

### Delete Item (Admin Only)

```bash
curl -X DELETE https://api.example.com/api/items/123e4567-e89b-12d3-a456-426614174000 \
  -H "Authorization: Bearer ${TOKEN}"
```

### List Items (Paginated)

```bash
curl "https://api.example.com/api/items?page=1&pageSize=10" \
  -H "Authorization: Bearer ${TOKEN}"
```

## 🔧 Customization Guide

### Adding New Features

1. **Create Feature Folder**: `Features/YourFeature/`
2. **Add Entity**: `YourEntity.cs`
3. **Add Requests/Responses**: `Requests.cs`, `Responses.cs`
4. **Add Validation**: `YourFeatureValidator.cs`
5. **Add Repository**: `YourRepository.cs`
6. **Add Endpoints**: `YourEndpoints.cs`
7. **Register in DbContext**: Update `ApplicationDbContext.cs`
8. **Add Migration**: `dotnet ef migrations add AddYourFeature`

### Switching Database Providers

**To SQL Server:**
1. Update `appsettings.json`: Set `DatabaseProvider` to `"SqlServer"`
2. Update connection string
3. Run migrations: `dotnet ef database update`

**To MongoDB (requires additional setup):**
1. Install MongoDB driver
2. Create MongoDB context
3. Update dependency injection
4. Implement repository pattern for MongoDB

### Adding Custom Middleware

```csharp
// Create middleware
public class CustomMiddleware
{
    private readonly RequestDelegate _next;

    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Before request
        await _next(context);
        // After request
    }
}

// Register in Program.cs
app.UseMiddleware();
```

## 📦 NuGet Packages Reference

### Core
- Microsoft.AspNetCore.App (Framework)
- Microsoft.EntityFrameworkCore 8.0.0
- Microsoft.EntityFrameworkCore.Design 8.0.0

### Database
- Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0
- Microsoft.EntityFrameworkCore.SqlServer 8.0.0

### Caching
- StackExchange.Redis 2.7.10
- Microsoft.Extensions.Caching.StackExchangeRedis 8.0.0

### Authentication
- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0

### Messaging
- MassTransit 8.1.3
- MassTransit.Azure.ServiceBus.Core 8.1.3
- MassTransit.RabbitMQ 8.1.3

### Validation
- FluentValidation 11.9.0
- FluentValidation.AspNetCore 11.3.0

### Logging & Monitoring
- Serilog.AspNetCore 8.0.0
- Microsoft.ApplicationInsights.AspNetCore 2.22.0

### Testing
- xUnit 2.6.2
- FluentAssertions 6.12.0
- Testcontainers.PostgreSql 3.6.0
- Microsoft.AspNetCore.Mvc.Testing 8.0.0

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

## 📝 License

MIT License - See LICENSE file for details

## 📞 Support

- **Documentation**: [Wiki](../../wiki)
- **Issues**: [GitHub Issues](../../issues)
- **Discussions**: [GitHub Discussions](../../discussions)
- **Email**: support@example.com

## 🙏 Acknowledgments

- .NET Team for the amazing framework
- Keycloak for identity management
- MassTransit for messaging abstractions
- Testcontainers for integration testing
- Community contributors

---

**Happy Coding! 🚀**# Production-Ready .NET 8 Backend Template

## 🚀 Quick Start

```bash
# Clone and initialize
git clone 
cd src/rapidCRUD

# Start with Docker Compose (includes PostgreSQL, Redis, Keycloak, RabbitMQ)
docker-compose up -d

# Run migrations
dotnet ef database update

# Start the API
dotnet run

# API available at: https://localhost:7001
# Swagger UI: https://localhost:7001/swagger
# Health checks: https://localhost:7001/health
```

## 📁 Project Structure

```
ProductionTemplate/
├── src/
│   ├── rapidCRUD/              # Main API project
│   │   ├── Features/               # Vertical slices
│   │   │   ├── Items/
│   │   │   │   ├── CreateItem.cs
│   │   │   │   ├── GetItem.cs
│   │   │   │   ├── UpdateItem.cs
│   │   │   │   └── DeleteItem.cs
│   │   │   └── Users/
│   │   ├── Infrastructure/
│   │   │   ├── Database/
│   │   │   ├── Messaging/
│   │   │   ├── Authentication/
│   │   │   └── Monitoring/
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── rapidCRUD.Contracts/    # Shared contracts
│   └── rapidCRUD.Tests/        # Test project
├── infrastructure/
│   ├── terraform/
│   │   ├── azure/
│   │   └── aws/
│   └── docker/
├── .github/workflows/              # GitHub Actions
├── azure-pipelines.yml             # Azure DevOps
└── docker-compose.yml
```

## 🏗️ Architecture

### Vertical Slice Architecture

Each feature is self-contained with:
- Request/Response models
- Validation
- Business logic
- Database access
- Event publishing

### Key Technologies

- **.NET 8** - Latest LTS framework
- **PostgreSQL** - Primary database
- **Redis** - Caching layer
- **Keycloak** - Identity & authentication
- **Azure Service Bus / RabbitMQ** - Messaging
- **MassTransit** - Messaging abstraction
- **Application Insights / DataDog** - Monitoring
- **Terraform** - Infrastructure as Code
- **Testcontainers** - Integration testing

## 🔐 Authentication & Authorization

### Keycloak Integration

```csharp
// Configured in Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
    });

// Usage in endpoints
app.MapGet("/api/items", [Authorize(Roles = "user")] 
    (IMediator mediator) => ...);
```

### OAuth2 Flows Supported

- Authorization Code Flow
- Client Credentials Flow
- Resource Owner Password Flow

## 💾 Database Configuration

### PostgreSQL (Primary)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=productiondb;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=5;MaxPoolSize=100"
  }
}
```

### SQL Server (Alternative)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=productiondb;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### Redis Configuration

```json
{
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "rapidCRUD:"
  }
}
```

### Migrations

```bash
# Add migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Generate SQL script
dotnet ef migrations script
```

## 📨 Messaging

### Azure Service Bus (Primary)

```json
{
  "AzureServiceBus": {
    "ConnectionString": "Endpoint=sb://...",
    "QueueName": "items-queue"
  }
}
```

### RabbitMQ (Fallback)

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Publishing Events

```csharp
public class ItemCreatedEvent
{
    public Guid ItemId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Publish via MassTransit
await publishEndpoint.Publish(new ItemCreatedEvent
{
    ItemId = item.Id,
    Name = item.Name,
    CreatedAt = DateTime.UtcNow
});
```

## 🏥 Health Checks

Configured endpoints:

- `/health` - Overall health status
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

Checks include:
- Database connectivity
- Redis availability
- Message bus connection
- External service dependencies

## 📊 Monitoring

### Application Insights

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=..."
  }
}
```

### DataDog

```json
{
  "DataDog": {
    "ApiKey": "your-api-key",
    "ServiceName": "production-api"
  }
}
```

## 🐳 Docker Deployment

### Build Image

```bash
docker build -t production-api:latest -f Dockerfile .
```

### Run Container

```bash
docker run -d \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="..." \
  production-api:latest
```

## ☁️ Cloud Deployment

### Azure (One-Command)

```bash
cd infrastructure/terraform/azure
terraform init
terraform apply -auto-approve

# Deploys:
# - Azure Container Apps
# - PostgreSQL Flexible Server
# - Azure Service Bus
# - Keycloak on Container Instance
# - Application Insights
# - Managed Identity
```

### AWS

```bash
cd infrastructure/terraform/aws
terraform init
terraform apply -auto-approve

# Deploys:
# - ECS Fargate
# - RDS PostgreSQL
# - ElastiCache Redis
# - Application Load Balancer
# - CloudWatch
```

## 🧪 Testing

### Run All Tests

```bash
dotnet test
```

### Test Structure

```
Tests/
├── Unit/                   # Fast, isolated tests
├── Integration/            # Database & external services
│   ├── DatabaseTests.cs
│   ├── MessagingTests.cs
│   └── AuthTests.cs
└── E2E/                    # Full system tests
```

### Testcontainers Example

```csharp
[Collection("Database")]
public class ItemTests : IAsyncLifetime
{
    private PostgreSqlContainer _dbContainer;
    
    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .Build();
        await _dbContainer.StartAsync();
    }
}
```

## 🔄 CI/CD

### GitHub Actions

Automatically triggers on:
- Push to main
- Pull requests
- Manual workflow dispatch

Pipeline includes:
1. Build & test
2. Docker image creation
3. Push to container registry
4. Deploy to Azure/AWS
5. Run smoke tests

### Azure DevOps

```yaml
# azure-pipelines.yml
trigger:
  - main

stages:
  - stage: Build
  - stage: Test
  - stage: Deploy
```

## 🌐 .NET Aspire Orchestration

```bash
# Run with Aspire
cd src/rapidCRUD.AppHost
dotnet run

# Aspire Dashboard: http://localhost:15888
```

Includes:
- Service discovery
- Distributed tracing
- Metrics dashboard
- Resource management

## ⚙️ Configuration by Environment

### Development (appsettings.Development.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "UseLocalEmulators": true
}
```

### Production (appsettings.Production.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "UseAzureManagedIdentity": true
}
```

## 🔧 Environment Variables

```bash
# Database
DATABASE_CONNECTION_STRING=...

# Authentication
KEYCLOAK_AUTHORITY=https://keycloak.example.com/realms/production
KEYCLOAK_CLIENT_ID=production-api
KEYCLOAK_CLIENT_SECRET=...

# Messaging
AZURE_SERVICE_BUS_CONNECTION_STRING=...
# OR
RABBITMQ_HOST=...

# Monitoring
APPLICATIONINSIGHTS_CONNECTION_STRING=...
DATADOG_API_KEY=...
```

## 📦 NuGet Packages

Key dependencies:
- `Microsoft.EntityFrameworkCore.PostgreSQL`
- `MassTransit.Azure.ServiceBus.Core`
- `MassTransit.RabbitMQ`
- `StackExchange.Redis`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.ApplicationInsights.AspNetCore`
- `Testcontainers.PostgreSql`
- `FluentValidation`
- `Serilog.AspNetCore`

## 🚦 API Examples

### Create Item

```bash
curl -X POST https://localhost:7001/api/items \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Sample Item",
    "description": "This is a sample item"
  }'
```

### Get Item

```bash
curl https://localhost:7001/api/items/{id} \
  -H "Authorization: Bearer {token}"
```

## 📝 License

MIT License - See LICENSE file for details

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Commit changes
4. Push to branch
5. Create pull request

## 📞 Support

- Documentation: [Wiki](wiki)
- Issues: [GitHub Issues](issues)
- Discussions: [GitHub Discussions](discussions)
