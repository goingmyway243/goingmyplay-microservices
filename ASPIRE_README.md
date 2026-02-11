# GoingMyPlay Microservices with .NET Aspire

This solution demonstrates a microservices architecture using .NET Aspire for orchestration, observability, and service discovery.

## Architecture

```
???????????????????????????????????????????
?         .NET Aspire AppHost             ?
?     (Orchestrator & Dashboard)          ?
???????????????????????????????????????????
                  ?
    ?????????????????????????????
    ?             ?             ?
    ?             ?             ?
??????????   ??????????   ??????????
?Identity?   ?Catalog ?   ?Payment ?
?Service ?   ?Service ?   ?Service ?
??????????   ??????????   ??????????
                  ?
                  ?
            ????????????????
            ? Elasticsearch?
            ????????????????
```

## Projects

### 1. **GoingMyPlay.AppHost** - Aspire Orchestrator
- Orchestrates all microservices
- Provides service discovery
- Manages Elasticsearch container
- Exposes Aspire Dashboard for observability

### 2. **GoingMyPlay.ServiceDefaults** - Shared Configuration
- OpenTelemetry instrumentation (metrics, tracing, logging)
- Health checks
- HTTP resilience patterns
- Service discovery

### 3. **Play.Identity** - Identity Service
- IdentityServer implementation
- JWT token generation
- User authentication
- Port: 7164 (HTTPS)

### 4. **Play.Catalog** - Catalog Service  
- Product/Item management
- Elasticsearch integration for search
- JWT authentication via Identity service
- RESTful API with Swagger

### 5. **Play.Payment** - Payment Service
- Payment processing
- JWT authentication via Identity service
- RESTful API with Swagger

## Getting Started

### Prerequisites
- .NET 9 SDK
- Docker Desktop (for Elasticsearch)
- Visual Studio 2022 or VS Code

### Running the Application

#### Option 1: Run with Aspire AppHost (Recommended)

```bash
cd src/GoingMyPlay.AppHost
dotnet run
```

This will:
1. Start the Aspire Dashboard
2. Launch all microservices
3. Start Elasticsearch in Docker
4. Configure service discovery automatically

**Aspire Dashboard:** https://localhost:17164

#### Option 2: Run Individual Services

```bash
# Terminal 1 - Identity Service
cd src/Play.Identity
dotnet run

# Terminal 2 - Catalog Service  
cd src/Play.Catalog
dotnet run

# Terminal 3 - Payment Service
cd src/Play.Payment
dotnet run

# Terminal 4 - Elasticsearch (Docker)
docker run -p 9200:9200 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:8.11.0
```

## Service Endpoints

### Aspire Dashboard
- **URL:** https://localhost:17164
- **Features:**
  - Service health monitoring
  - Distributed tracing
  - Metrics visualization
  - Logs aggregation
  - Container management

### Play.Identity
- **Swagger:** https://localhost:7164/swagger
- **Endpoints:**
  - `GET /api/identity/public-key` - Get public key for JWT validation
  - IdentityServer endpoints for authentication

### Play.Catalog
- **Swagger:** https://localhost:XXXX/swagger
- **Endpoints:**
  - `GET /api/items` - Get all items
  - `GET /api/items/{id}` - Get item by ID
  - `GET /api/items/search?query={text}` - Search items
  - `POST /api/items` - Create item (requires auth)
  - `PUT /api/items/{id}` - Update item (requires auth)
  - `DELETE /api/items/{id}` - Delete item (requires auth)
- **Health:** 
  - `/health` - Overall health
  - `/alive` - Liveness probe

### Play.Payment
- **Swagger:** https://localhost:XXXX/swagger
- **Endpoints:**
  - Payment endpoints (requires auth)
- **Health:**
  - `/health` - Overall health
  - `/alive` - Liveness probe

### Elasticsearch
- **URL:** http://localhost:9200
- **Index:** `catalog-items`

## Observability Features

### 1. **Distributed Tracing**
- Automatic trace propagation across services
- View end-to-end request flows in Aspire Dashboard
- W3C Trace Context standard

### 2. **Metrics**
- HTTP request metrics
- Runtime metrics (GC, memory, CPU)
- Custom business metrics
- Real-time visualization

### 3. **Logging**
- Structured logging with OpenTelemetry
- Centralized log aggregation
- Log correlation with traces
- Log levels and filtering

### 4. **Health Checks**
- `/health` - Overall health status
- `/alive` - Liveness probe for Kubernetes
- Automatic service health monitoring

## Service Discovery

Services automatically discover each other using Aspire's service discovery:

```csharp
// Catalog service discovering Identity service
var identityUrl = builder.Configuration.GetConnectionString("identity");

// Catalog service discovering Elasticsearch
var elasticsearchUrl = builder.Configuration.GetConnectionString("elasticsearch");
```

No hard-coded URLs needed!

## Resilience Patterns

Built-in resilience with `Microsoft.Extensions.Http.Resilience`:
- **Retry** - Automatic retries with exponential backoff
- **Circuit Breaker** - Prevent cascade failures
- **Timeout** - Request timeouts
- **Rate Limiting** - Protect against overload

## Development Workflow

### 1. **Local Development**
```bash
# Run with hot reload
dotnet watch --project src/GoingMyPlay.AppHost
```

### 2. **Running Tests**
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 3. **Building**
```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/Play.Catalog
```

## Configuration

### AppHost Configuration
Edit `src/GoingMyPlay.AppHost/Program.cs` to:
- Add/remove services
- Configure container resources
- Set environment variables
- Configure networking

### Service Configuration
Each service has its own `appsettings.json`:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Dev overrides

### Connection Strings
Aspire automatically injects connection strings:
- `elasticsearch` - Elasticsearch URL
- `identity` - Identity service URL

## Deployment

### Container Images
```bash
# Build container images
dotnet publish -c Release

# Or use Docker
docker build -t play-catalog:latest -f src/Play.Catalog/Dockerfile .
```

### Kubernetes
Aspire can generate Kubernetes manifests:
```bash
# Generate K8s manifests
dotnet publish --os linux --arch x64 /t:PublishContainer
```

## Troubleshooting

### Services Not Starting
- Check Docker Desktop is running
- Verify ports are not in use
- Check Aspire Dashboard logs

### Elasticsearch Connection Issues
- Verify Elasticsearch container is running
- Check port 9200 is accessible
- Review Elasticsearch logs in Aspire Dashboard

### Authentication Failures
- Ensure Identity service is running
- Check JWT token expiration
- Verify audience and issuer configuration

## Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [OpenTelemetry](https://opentelemetry.io/)
- [IdentityServer](https://duendesoftware.com/products/identityserver)
- [Elasticsearch](https://www.elastic.co/elasticsearch/)

## Project Structure

```
goingmyplay-microservices/
??? src/
?   ??? GoingMyPlay.AppHost/          # Aspire orchestrator
?   ??? GoingMyPlay.ServiceDefaults/  # Shared configuration
?   ??? Play.Identity/                # Identity service
?   ??? Play.Catalog/                 # Catalog service
?   ??? Play.Payment/                 # Payment service
??? tests/
?   ??? Play.Catalog.Tests/           # Unit tests
??? README.md
```

## Next Steps

1. ? Add more microservices
2. ? Implement message queuing (RabbitMQ/Azure Service Bus)
3. ? Add API Gateway (YARP)
4. ? Implement caching (Redis)
5. ? Add database per service
6. ? Implement event-driven architecture
7. ? Add integration tests
8. ? Set up CI/CD pipeline
9. ? Deploy to Azure Container Apps

## License

MIT License
