# ?? Run Your Play Microservices in 3 Steps

## Step 1: Start the Orchestrator

```bash
cd src/Play.AppHost
dotnet run
```

**What happens:**
```
? Starting Identity Service...
? Starting Catalog Service...
? Starting Payment Service...
? Pulling Elasticsearch container...
? Starting Elasticsearch...
? Configuring service discovery...
? Starting Aspire Dashboard...

Now listening on: https://localhost:17164
```

## Step 2: Open the Aspire Dashboard

Navigate to: **https://localhost:17164**

**Dashboard Tabs:**

### ?? Resources
```
??????????????????????????????????????????????
? Service          ? Status   ? Endpoints    ?
??????????????????????????????????????????????
? identity         ? Running  ? :7164       ?
? catalog          ? Running  ? :xxxxx      ?
? payment          ? Running  ? :xxxxx      ?
? elasticsearch    ? Running  ? :9200       ?
??????????????????????????????????????????????
```

### ?? Console Logs
Real-time logs from all services with filtering

### ?? Traces
Distributed tracing - see requests flow across services

### ?? Metrics
HTTP requests, response times, error rates

### ?? Structured Logs
Detailed logging with search and filtering

## Step 3: Test Your Services

### Option A: Use Swagger

1. Click on **catalog** in Resources tab
2. Click on the HTTPS endpoint
3. Add `/swagger` to the URL
4. Try the APIs!

### Option B: Use cURL

```bash
# Get all catalog items
curl -X GET "https://localhost:<port>/api/items" -k

# Search items
curl -X GET "https://localhost:<port>/api/items/search?query=sword" -k

# Check health
curl -X GET "https://localhost:<port>/health" -k
```

### Option C: Use Postman

Import this collection:
```json
{
  "info": {
    "name": "Play Microservices"
  },
  "item": [
    {
      "name": "Get All Items",
      "request": {
        "method": "GET",
        "url": "https://localhost:{{catalog-port}}/api/items"
      }
    }
  ]
}
```

## ?? You're Done!

Your entire microservices architecture is running with:
- ? All 3 services
- ? Elasticsearch
- ? Service discovery
- ? Distributed tracing
- ? Health monitoring
- ? Centralized logging

## ?? Hot Tips

### View a Request Flow
1. Make an API request
2. Go to **Traces** tab in dashboard
3. Click on your trace
4. See the entire request journey!

### Check Service Health
- Green = Healthy
- Red = Unhealthy
- Click service for details

### View Logs
- Go to **Console Logs**
- Filter by service name
- Search for specific text
- Adjust log levels

### Restart a Service
1. Go to **Resources** tab
2. Click on service
3. Click "Restart"

## ?? Stopping

Press `Ctrl+C` in the terminal

This stops:
- All services
- Dashboard
- Elasticsearch container (keeps data)

## ?? Next Run

Just run: `dotnet run`

Everything starts again with your data intact!

## ?? Learn More

- [Full README](./ASPIRE_README.md)
- [Quick Start Guide](./ASPIRE_QUICK_START.md)
- [Implementation Details](./ASPIRE_IMPLEMENTATION_SUMMARY.md)

---

**That's it! Your cloud-ready microservices are running! ??**
