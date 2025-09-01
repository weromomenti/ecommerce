# Azure Infrastructure Setup Script for InventoryService (PowerShell)
# Run this script to set up all required Azure resources

# Configuration
$ResourceGroup = "Learning"
$Location = "East US"
$AcrName = "ecommercecontainerregiry"
$ContainerAppEnv = "az204-demo-env"
$SqlServerName = "ecommerce-sql-server"
$RedisName = "ecommerce-redis"
$ServiceBusNamespace = "ecommerce-servicebus"

Write-Host "?? Setting up Azure infrastructure for InventoryService..." -ForegroundColor Green

# Create Resource Group (if not exists)
Write-Host "?? Creating resource group..." -ForegroundColor Yellow
az group create --name $ResourceGroup --location $Location --output none

# Create Azure Container Registry
Write-Host "?? Creating Azure Container Registry..." -ForegroundColor Yellow
az acr create `
  --resource-group $ResourceGroup `
  --name $AcrName `
  --sku Basic `
  --location $Location `
  --output none

# Enable admin user for ACR
Write-Host "?? Enabling ACR admin user..." -ForegroundColor Yellow
az acr update --name $AcrName --admin-enabled true --output none

# Create Container Apps Environment
Write-Host "??? Creating Container Apps Environment..." -ForegroundColor Yellow
az containerapp env create `
  --name $ContainerAppEnv `
  --resource-group $ResourceGroup `
  --location $Location `
  --output none

# Create Azure SQL Server and Database
Write-Host "??? Creating Azure SQL Server..." -ForegroundColor Yellow
$SqlAdminPassword = "ComplexPassword123!"
az sql server create `
  --name $SqlServerName `
  --resource-group $ResourceGroup `
  --location $Location `
  --admin-user sqladmin `
  --admin-password $SqlAdminPassword `
  --output none

# Create SQL Database
Write-Host "?? Creating InventoryDb database..." -ForegroundColor Yellow
az sql db create `
  --resource-group $ResourceGroup `
  --server $SqlServerName `
  --name InventoryDb `
  --service-objective Basic `
  --output none

# Configure SQL Server firewall for Azure services
Write-Host "?? Configuring SQL Server firewall..." -ForegroundColor Yellow
az sql server firewall-rule create `
  --resource-group $ResourceGroup `
  --server $SqlServerName `
  --name AllowAzureServices `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 0.0.0.0 `
  --output none

# Create Azure Cache for Redis
Write-Host "?? Creating Azure Cache for Redis..." -ForegroundColor Yellow
az redis create `
  --resource-group $ResourceGroup `
  --name $RedisName `
  --location $Location `
  --sku Basic `
  --vm-size c0 `
  --output none

# Create Azure Service Bus
Write-Host "?? Creating Azure Service Bus..." -ForegroundColor Yellow
az servicebus namespace create `
  --resource-group $ResourceGroup `
  --name $ServiceBusNamespace `
  --location $Location `
  --sku Standard `
  --output none

# Create Service Bus topic
Write-Host "?? Creating orders topic..." -ForegroundColor Yellow
az servicebus topic create `
  --resource-group $ResourceGroup `
  --namespace-name $ServiceBusNamespace `
  --name orders `
  --output none

# Get connection strings and keys
Write-Host "?? Retrieving connection strings..." -ForegroundColor Yellow

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host "?? Infrastructure setup completed!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""

Write-Host "?? Add these to your GitHub Secrets:" -ForegroundColor Cyan
Write-Host ""

# ACR credentials
$AcrUsername = az acr credential show --name $AcrName --query "username" -o tsv
$AcrPassword = az acr credential show --name $AcrName --query "passwords[0].value" -o tsv
Write-Host "ACR_USERNAME: $AcrUsername" -ForegroundColor White
Write-Host "ACR_PASSWORD: $AcrPassword" -ForegroundColor White
Write-Host ""

# SQL connection string
$SqlConnectionString = "Server=$SqlServerName.database.windows.net;Database=InventoryDb;User Id=sqladmin;Password=$SqlAdminPassword;TrustServerCertificate=true;Connection Timeout=30;"
Write-Host "INVENTORY_SQL_CONNECTION_STRING:" -ForegroundColor White
Write-Host $SqlConnectionString -ForegroundColor White
Write-Host ""

# Redis connection string
$RedisKey = az redis list-keys --resource-group $ResourceGroup --name $RedisName --query "primaryKey" -o tsv
$RedisConnectionString = "$RedisName.redis.cache.windows.net:6380,password=$RedisKey,ssl=True,abortConnect=False"
Write-Host "INVENTORY_REDIS_CONNECTION_STRING:" -ForegroundColor White
Write-Host $RedisConnectionString -ForegroundColor White
Write-Host ""

# Service Bus connection string
$ServiceBusConnectionString = az servicebus namespace authorization-rule keys list --resource-group $ResourceGroup --namespace-name $ServiceBusNamespace --name RootManageSharedAccessKey --query "primaryConnectionString" -o tsv
Write-Host "RABBITMQ_HOST (Service Bus connection):" -ForegroundColor White
Write-Host $ServiceBusConnectionString -ForegroundColor White
Write-Host ""

# MongoDB (you'll need to set this up separately or use Azure Cosmos DB)
Write-Host "INVENTORY_MONGODB_CONNECTION_STRING:" -ForegroundColor White
Write-Host "(Set up MongoDB Atlas or Azure Cosmos DB for MongoDB API)" -ForegroundColor Yellow
Write-Host ""

Write-Host "==========================================" -ForegroundColor Green
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Add all the above secrets to your GitHub repository" -ForegroundColor White
Write-Host "2. Set up MongoDB (Atlas or Cosmos DB)" -ForegroundColor White
Write-Host "3. Push your code to trigger the deployment workflow" -ForegroundColor White
Write-Host "==========================================" -ForegroundColor Green