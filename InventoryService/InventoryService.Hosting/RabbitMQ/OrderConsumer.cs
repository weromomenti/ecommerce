using Ecommerce.Library.Messaging;
using InventoryService.Business.Entities;
using InventoryService.Business.Interfaces;
using MassTransit;

namespace InventoryService.Hosting.RabbitMQ
{
    public class OrderConsumer(
        ILogger<OrderConsumer> logger,
        IInventoryManagementService inventoryService) : IConsumer<OrderContract>
    {

        public async Task Consume(ConsumeContext<OrderContract> context)
        {
            var msg = context.Message;

            try
            {
                switch (msg.eventType)
                {
                    case EventType.Created:
                        logger.LogInformation("Processing created order message");
                        await inventoryService.ProcessCreateOrderAsync(msg.order);
                        break;

                    case EventType.Updated:
                        logger.LogInformation("Processing updated order message");
                        await inventoryService.ProcessUpdateOrderAsync(msg.order);
                        break;

                    case EventType.Deleted:
                        logger.LogInformation("Processing deleted order message");
                        await inventoryService.ProcessCancelOrderAsync(msg.order);
                        break;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}