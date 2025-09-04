# GitHub Secrets Configuration for InventoryService Deployment

## Required GitHub Secrets

You need to add the following secrets to your GitHub repository:

### Azure Authentication
- `AZURE_CREDENTIALS` - Azure service principal credentials (already exists)
- `ACR_USERNAME` - Azure Container Registry username
- `ACR_PASSWORD` - Azure Container Registry password

### Database Connection Strings
- `INVENTORY_SQL_CONNECTION_STRING` - SQL Server connection string for InventoryService
  ```
  Server=your-sql-server.database.windows.net;Database=InventoryDb;User Id=your-username;Password=your-password;TrustServerCertificate=true;Connection Timeout=30;
  ```

- `INVENTORY_MONGODB_CONNECTION_STRING` - MongoDB connection string
  ```
  mongodb://your-mongodb-connection-string/
  ```

- `INVENTORY_REDIS_CONNECTION_STRING` - Redis connection string
  ```
  your-redis-cache.redis.cache.windows.net:6380,password=your-redis-key,ssl=True,abortConnect=False
  ```

### Message Queue Configuration
- `RABBITMQ_HOST` - RabbitMQ/Azure Service Bus connection string
  ```
  Endpoint=sb://your-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key
  ```

## How to Add Secrets

1. Go to your GitHub repository
2. Navigate to Settings ? Secrets and variables ? Actions
3. Click "New repository secret"
4. Add each secret with the exact name and value

## Azure Resources Required

### Azure Container Registry
```bash
# Create ACR if not exists
az acr create --resource-group Learning --name ecommercecontainerregiry --sku Basic
```

### Azure Container Apps Environment
```bash
# Create Container Apps Environment if not exists
az containerapp env create \
  --name az204-demo-env \
  --resource-group Learning \
  --location "East US"
```

### Azure SQL Database
- Create database named `InventoryDb`
- Configure firewall rules for Azure services

### Azure Cache for Redis
- Create Redis cache instance
- Get connection string from portal

### Azure Service Bus (Alternative to RabbitMQ)
- Create Service Bus namespace
- Create topic named "orders"
- Get connection string

## Environment Variables Explanation

### InventoryService API
- `ASPNETCORE_ENVIRONMENT=Release` - ASP.NET Core environment
- `ConnectionStrings__DefaultConnection` - SQL Server for inventory data
- `ConnectionStrings__MongoDb` - MongoDB for audit trails
- `ConnectionStrings__Redis` - Redis for caching

### InventoryService Worker
- `DOTNET_ENVIRONMENT=Release` - .NET environment
- `RabbitMQ__Host` - Message queue connection
- Same database connections as API

## Deployment Triggers

The workflow triggers on:
1. Push to main branch with changes in `InventoryService/**`
2. Manual workflow dispatch

## Container Apps Configuration

### API Container
- External ingress (publicly accessible)
- Port 8080
- 1-3 replicas (auto-scaling)
- 0.5 CPU, 1Gi memory

### Worker Container
- Internal ingress (not publicly accessible)
- 1-2 replicas
- 0.25 CPU, 0.5Gi memory