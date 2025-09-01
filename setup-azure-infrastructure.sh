#!/bin/bash

# Azure Infrastructure Setup Script for InventoryService
# Run this script to set up all required Azure resources

set -e

# Configuration
RESOURCE_GROUP="Learning"
LOCATION="East US"
ACR_NAME="ecommercecontainerregiry"
CONTAINER_APP_ENV="az204-demo-env"
SQL_SERVER_NAME="ecommerce-sql-server"
REDIS_NAME="ecommerce-redis"
SERVICEBUS_NAMESPACE="ecommerce-servicebus"

echo "?? Setting up Azure infrastructure for InventoryService..."

# Create Resource Group (if not exists)
echo "?? Creating resource group..."
az group create --name $RESOURCE_GROUP --location "$LOCATION" --output none

# Create Azure Container Registry
echo "?? Creating Azure Container Registry..."
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Basic \
  --location "$LOCATION" \
  --output none

# Enable admin user for ACR
echo "?? Enabling ACR admin user..."
az acr update --name $ACR_NAME --admin-enabled true --output none

# Create Container Apps Environment
echo "??? Creating Container Apps Environment..."
az containerapp env create \
  --name $CONTAINER_APP_ENV \
  --resource-group $RESOURCE_GROUP \
  --location "$LOCATION" \
  --output none

# Create Azure SQL Server and Database
echo "??? Creating Azure SQL Server..."
SQL_ADMIN_PASSWORD="ComplexPassword123!"
az sql server create \
  --name $SQL_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --location "$LOCATION" \
  --admin-user sqladmin \
  --admin-password $SQL_ADMIN_PASSWORD \
  --output none

# Create SQL Database
echo "?? Creating InventoryDb database..."
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name InventoryDb \
  --service-objective Basic \
  --output none

# Configure SQL Server firewall for Azure services
echo "?? Configuring SQL Server firewall..."
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0 \
  --output none

# Create Azure Cache for Redis
echo "?? Creating Azure Cache for Redis..."
az redis create \
  --resource-group $RESOURCE_GROUP \
  --name $REDIS_NAME \
  --location "$LOCATION" \
  --sku Basic \
  --vm-size c0 \
  --output none

# Create Azure Service Bus
echo "?? Creating Azure Service Bus..."
az servicebus namespace create \
  --resource-group $RESOURCE_GROUP \
  --name $SERVICEBUS_NAMESPACE \
  --location "$LOCATION" \
  --sku Standard \
  --output none

# Create Service Bus topic
echo "?? Creating orders topic..."
az servicebus topic create \
  --resource-group $RESOURCE_GROUP \
  --namespace-name $SERVICEBUS_NAMESPACE \
  --name orders \
  --output none

# Get connection strings and keys
echo "?? Retrieving connection strings..."

echo ""
echo "=========================================="
echo "?? Infrastructure setup completed!"
echo "=========================================="
echo ""

echo "?? Add these to your GitHub Secrets:"
echo ""

# ACR credentials
ACR_USERNAME=$(az acr credential show --name $ACR_NAME --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name $ACR_NAME --query "passwords[0].value" -o tsv)
echo "ACR_USERNAME: $ACR_USERNAME"
echo "ACR_PASSWORD: $ACR_PASSWORD"
echo ""

# SQL connection string
SQL_CONNECTION_STRING="Server=$SQL_SERVER_NAME.database.windows.net;Database=InventoryDb;User Id=sqladmin;Password=$SQL_ADMIN_PASSWORD;TrustServerCertificate=true;Connection Timeout=30;"
echo "INVENTORY_SQL_CONNECTION_STRING:"
echo "$SQL_CONNECTION_STRING"
echo ""

# Redis connection string
REDIS_KEY=$(az redis list-keys --resource-group $RESOURCE_GROUP --name $REDIS_NAME --query "primaryKey" -o tsv)
REDIS_CONNECTION_STRING="$REDIS_NAME.redis.cache.windows.net:6380,password=$REDIS_KEY,ssl=True,abortConnect=False"
echo "INVENTORY_REDIS_CONNECTION_STRING:"
echo "$REDIS_CONNECTION_STRING"
echo ""

# Service Bus connection string
SERVICEBUS_CONNECTION_STRING=$(az servicebus namespace authorization-rule keys list --resource-group $RESOURCE_GROUP --namespace-name $SERVICEBUS_NAMESPACE --name RootManageSharedAccessKey --query "primaryConnectionString" -o tsv)
echo "RABBITMQ_HOST (Service Bus connection):"
echo "$SERVICEBUS_CONNECTION_STRING"
echo ""

# MongoDB (you'll need to set this up separately or use Azure Cosmos DB)
echo "INVENTORY_MONGODB_CONNECTION_STRING:"
echo "(Set up MongoDB Atlas or Azure Cosmos DB for MongoDB API)"
echo ""

echo "=========================================="
echo "Next Steps:"
echo "1. Add all the above secrets to your GitHub repository"
echo "2. Set up MongoDB (Atlas or Cosmos DB)"
echo "3. Push your code to trigger the deployment workflow"
echo "=========================================="