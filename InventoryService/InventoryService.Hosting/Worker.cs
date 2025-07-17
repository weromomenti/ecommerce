using InventoryService.Hosting.RabbitMQ;
using Microsoft.Extensions.Logging;

namespace InventoryService.Hosting
{
    public class Worker(ILogger<Worker> logger, IOrderConsumer orderConsumer) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting consuming");
            orderConsumer.StartConsumingAsync();

            return Task.CompletedTask;
        }
    }
}
