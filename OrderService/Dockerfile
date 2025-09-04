FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for restore
COPY ["OrderService/OrderService.csproj", "OrderService/"]
COPY ["Ecommerce.Library/Ecommerce.Library.csproj", "Ecommerce.Library/"]

RUN dotnet restore "OrderService/OrderService.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/OrderService"
RUN dotnet build "OrderService.csproj" -c Release -o /app/build

# Publish stage
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderService.dll"]