using InventoryService.Business.Interfaces;
using InventoryService.Persistance.Infrastructure;
using InventoryService.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Persistance.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
        {
            // Register AutoMapper with persistence mapping profile
            services.AddAutoMapper(typeof(PersistenceMappingProfile));

            // Register DbContext for SQL Server
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            // Register MongoDB context
            services.AddSingleton(_ =>
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                var databaseName = configuration["MongoDb:DatabaseName"];
                return new MongoDbContext(connectionString ?? throw new InvalidOperationException("MongoDb connection string is required"), 
                                        databaseName ?? throw new InvalidOperationException("MongoDb database name is required"));
            });

            return services;
        }
    }
}