using InventoryService.Business.Interfaces;
using InventoryService.Persistance.Infrastructure;
using InventoryService.Persistance.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace InventoryService.Persistance.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services, IHostApplicationBuilder builder)
        {
            // Register AutoMapper with persistence mapping profile
            services.AddAutoMapper(typeof(PersistenceMappingProfile));

            // Register DbContext for SQL Server
            if (builder.Environment.IsProduction())
            {
                services.AddDbContext<InventoryDbContext>(options =>
                {
                    options.UseAzureSql(builder.Configuration.GetConnectionString("DbConnection"));
                });
            }
            else
            {
                services.AddDbContext<InventoryDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
            }

            // Register repositories
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            // Register MongoDB context
            services.AddSingleton(_ =>
            {
                var connectionString = builder.Configuration.GetConnectionString("MongoDb");
                var databaseName = builder.Configuration["MongoDb:DatabaseName"];
                
                // If database name not configured, try to extract from connection string
                if (string.IsNullOrEmpty(databaseName) && !string.IsNullOrEmpty(connectionString))
                {
                    var mongoUrl = new MongoUrl(connectionString);
                    databaseName = mongoUrl.DatabaseName;
                }
                
                return new MongoDbContext(connectionString ?? throw new InvalidOperationException("MongoDb connection string is required"), 
                                        databaseName ?? throw new InvalidOperationException("MongoDb database name is required"));
            });

            return services;
        }
    }
}