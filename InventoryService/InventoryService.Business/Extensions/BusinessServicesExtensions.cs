using InventoryService.Business.Interfaces;
using InventoryService.Business.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Business.Extensions
{
    public static class BusinessServiceExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register business services
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var configurationOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"), true);

                return ConnectionMultiplexer.Connect(configurationOptions);
            });
            services.AddScoped<IInventoryManagementService, InventoryManagementService>();
            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}
