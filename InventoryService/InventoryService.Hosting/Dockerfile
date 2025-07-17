# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["/InventoryService.Hosting/InventoryService.Hosting.csproj", "InventoryService.Hosting/"]
COPY ["/InventoryService.Business/InventoryService.Business.csproj", "InventoryService.Business/"]
COPY ["/InventoryService.Persistance/InventoryService.Persistance.csproj", "InventoryService.Persistance/"]

RUN dotnet restore InventoryService.Hosting/InventoryService.Hosting.csproj

# Copy all source code
COPY . ./

# Build the InventoryService
RUN dotnet publish InventoryService.Hosting/InventoryService.Hosting.csproj -c Release -o out

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && apt-get install -y libkrb5-3
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "InventoryService.Hosting.dll"]
