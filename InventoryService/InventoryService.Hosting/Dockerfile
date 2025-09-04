## Runtime base (includes Microsoft.AspNetCore.App shared framework)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

## Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files for restore (paths relative to solution root context)
COPY ["InventoryService/InventoryService.Hosting/InventoryService.Hosting.csproj", "InventoryService.Hosting/"]
COPY ["InventoryService/InventoryService.Business/InventoryService.Business.csproj", "InventoryService.Business/"]
COPY ["InventoryService/InventoryService.Persistance/InventoryService.Persistance.csproj", "InventoryService.Persistance/"]
COPY ["Ecommerce.Library/Ecommerce.Library.csproj", "Ecommerce.Library/"]

RUN dotnet restore "InventoryService/InventoryService.Hosting/InventoryService.Hosting.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/InventoryService/InventoryService.Hosting"
RUN dotnet build "InventoryService.Hosting.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/InventoryService/InventoryService.Hosting"
RUN dotnet publish "InventoryService.Hosting.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.Hosting.dll"]
