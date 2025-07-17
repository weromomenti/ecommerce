using InventoryService.Business.Extensions;
using InventoryService.Hosting.RabbitMQ;
using InventoryService.Persistance.Extensions;
using InventoryService.Persistance.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InventoryService.Hosting
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Register business services
            builder.Services.AddBusinessServices(builder.Configuration);

            builder.Services.AddPersistance(builder.Configuration);

            builder.Services.AddSingleton<IOrderConsumer, OrderConsumer>();

            builder.Services.AddHostedService<Worker>();

            builder.Logging.AddSerilog();

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("https://seq:5341")
                .CreateLogger();

            builder.Logging.AddSerilog(logger);

            var host = builder.Build();
            
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            await dbContext.Database.MigrateAsync();
            InventoryDataSeeder.Seed(dbContext);

            await host.RunAsync();
        }
    }
}