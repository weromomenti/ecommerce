using InventoryService.Business.Extensions;
using InventoryService.Hosting.RabbitMQ;
using InventoryService.Persistance.Extensions;
using InventoryService.Persistance.Infrastructure;
using MassTransit;
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
            builder.Services.AddBusinessServices(builder);

            builder.Services.AddPersistance(builder);

            builder.Services.AddHostedService<Worker>();

            builder.Logging.AddSerilog();

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderConsumer>();
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(builder.Configuration.GetConnectionString("MessageBrokerConnection"));
                    cfg.DeployTopologyOnly = false;

                    cfg.Message

                    cfg.ReceiveEndpoint("order-queue", e =>
                    {
                        e.ConfigureConsumer<OrderConsumer>(context);
                    });
                });
            });

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