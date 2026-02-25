# .NET Aspire Orchestrator - Quick Start Guide

## What is .NET Aspire?

.NET Aspire is an opinionated, cloud-ready stack for building observable, production-ready, distributed applications. It provides:

- **Service Orchestration** - Run all your microservices with one command
- **Service Discovery** - Services automatically discover each other
- **Observability** - Built-in dashboard with metrics, traces, and logs
- **Container Management** - Automatically manages Docker containers (e.g., Elasticsearch)
- **Development Dashboard** - Real-time monitoring during development

## What We Added

### 1. Play.AppHost (Orchestrator)
Located in: `src/Play.AppHost/`

This project orchestrates all services:
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Elasticsearch container
var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Identity Service
var identity = builder.AddProject<Projects.Play_Identity>("identity")
    .WithExternalHttpEndpoints()
    .WithHttpsEndpoint(port: 7164, name: "https");

// Catalog Service (references Elasticsearch and Identity)
var catalog = builder.AddProject<Projects.Play_Catalog>("catalog")
    .WithReference(elasticsearch)
    .WithReference(identity)
    .WithExternalHttpEndpoints();

// Payment Service
var payment = builder.AddProject<Projects.Play_Payment>("payment")
    .WithReference(identity)
    .WithExternalHttpEndpoints();
```

### 2. Play.ServiceDefaults (Shared Configuration)
Located in: `src/Play.ServiceDefaults/`

Provides shared functionality for all services:
- OpenTelemetry instrumentation
- Health checks (`/health` and `/alive` endpoints)
- HTTP resilience (retry, circuit breaker, timeout)
- Service discovery

### 3. Updated All Services

Each service now:
1. References ServiceDefaults project
2. Calls `builder.AddServiceDefaults()` for automatic setup
3. Calls `app.MapDefaultEndpoints()` for health checks
4. Uses service discovery for inter-service communication

## How to Run

### Step 1: Ensure Prerequisites
```bash
# Check .NET SDK
dotnet --version  # Should be 9.0 or higher

# Check Docker is running
docker ps
```

### Step 2: Run the AppHost
```bash
cd src/Play.AppHost
dotnet run
```

### Step 3: Open Aspire Dashboard
The console will display:
```
Now listening on: https://localhost:17164
Aspire Dashboard: https://localhost:17164
```

Open the dashboard in your browser!

## Aspire Dashboard Features

### 1. Resources Tab
- View all running services and containers
- See service status (Running, Starting, Stopped)
- View environment variables
- Access logs for each service
- Restart services

### 2. Console Logs Tab
- Real-time logs from all services
- Filter by service name
- Search logs
- Color-coded by log level

### 3. Structured Logs Tab
- Detailed structured logging
- Filter by timestamp, level, service
- View log properties

### 4. Traces Tab
- Distributed tracing across services
- View request flow between services
- Identify bottlenecks
- Click on trace to see details

### 5. Metrics Tab
- HTTP request metrics
- Response times
- Error rates
- Resource usage (CPU, memory)
- Custom metrics

## Service Discovery in Action

Before Aspire:
```csharp
// Hard-coded URLs ?
options.Authority = "https://localhost:7164";
var elasticsearchUrl = "http://localhost:9200";
```

With Aspire:
```csharp
// Dynamic service discovery ?
var identityUrl = builder.Configuration.GetConnectionString("identity");
var elasticsearchUrl = builder.Configuration.GetConnectionString("elasticsearch");
```

## Example Workflow

### 1. Start the Application
```bash
cd src/Play.AppHost
dotnet run
```

### 2. View Dashboard
Navigate to: https://localhost:17164

### 3. Make API Request
```bash
# Get all catalog items
curl https://localhost:<catalog-port>/api/items
```

### 4. View Trace
1. Go to Aspire Dashboard ? Traces tab
2. Find your request
3. See the entire request flow:
   - Catalog service receives request
   - Queries Elasticsearch
   - Returns response

### 5. View Metrics
1. Go to Metrics tab
2. See request count, duration, errors
3. Monitor in real-time

## Benefits for Your Project

### 1. **Simplified Development**
- One command to run everything
- No need to start each service manually
- Automatic container management

### 2. **Better Debugging**
- See logs from all services in one place
- Trace requests across services
- Identify issues quickly

### 3. **Production-Ready**
- Built-in observability
- Resilience patterns
- Service discovery
- Health checks

### 4. **Easy to Scale**
- Add new services easily
- Configure resources (CPU, memory)
- Environment-based configuration

## Adding a New Service

### Step 1: Create Service
```bash
dotnet new webapi -n Play.NewService -o src/Play.NewService
```

### Step 2: Add ServiceDefaults Reference
```bash
dotnet add src/Play.NewService reference src/Play.ServiceDefaults
```

### Step 3: Update Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();  // Add this

// ... your services

var app = builder.Build();

app.MapDefaultEndpoints();  // Add this

// ... your middleware

app.Run();
```

### Step 4: Add to AppHost
```csharp
// In src/Play.AppHost/Program.cs
var newService = builder.AddProject<Projects.Play_NewService>("newservice")
    .WithExternalHttpEndpoints();
```

### Step 5: Add Project Reference
```bash
dotnet add src/Play.AppHost reference src/Play.NewService
```

Done! Your new service is now orchestrated!

## Common Commands

```bash
# Run with hot reload
dotnet watch --project src/Play.AppHost

# Run tests
dotnet test

# Build everything
dotnet build

# Restore packages
dotnet restore

# Clean solution
dotnet clean
```

## Troubleshooting

### Dashboard Not Loading
- Check the console output for the correct URL
- Try http://localhost:15164 if HTTPS doesn't work
- Ensure no other application is using port 17164

### Services Not Starting
- Check Docker Desktop is running
- Verify ports are available
- Check console logs for errors

### Elasticsearch Container Issues
```bash
# Check if container is running
docker ps

# View container logs
docker logs <container-id>

# Remove and restart
docker stop <container-id>
docker rm <container-id>
```

### Service Discovery Not Working
- Ensure `builder.AddServiceDefaults()` is called
- Check service names in AppHost match configuration
- Verify connection string names are correct

## What's Next?

1. **Explore the Dashboard** - Familiarize yourself with all tabs
2. **Make Some Requests** - Use Swagger or Postman
3. **View Traces** - See how requests flow
4. **Check Metrics** - Monitor performance
5. **Add New Services** - Practice the workflow

## Key Files to Know

```
src/Play.AppHost/
??? Program.cs              # Service orchestration
??? Properties/
?   ??? launchSettings.json # Dashboard URL configuration
??? appsettings.json        # Logging configuration

src/Play.ServiceDefaults/
??? Extensions.cs           # Shared configuration
```

## Resources

- [.NET Aspire Docs](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Samples](https://github.com/dotnet/aspire-samples)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)

## Summary

? **One Command** - Start all services  
? **Automatic Discovery** - Services find each other  
? **Built-in Dashboard** - Monitor everything  
? **Production-Ready** - Observability included  
? **Easy to Extend** - Add services quickly  

You now have a modern, cloud-ready microservices architecture! ??
