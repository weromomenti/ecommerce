using System.Text;
using System.Text.Json;
using OrderService.Interfaces;
using OrderService.Entities;
using MassTransit;
using OrderService.Infrustructure;

namespace OrderService.Messaging
{
    public class OrderProducer(IPublishEndpoint bus) : IOrderProducer
    {
        public async Task SendOrderCreatedMessageAsync(OrderEntity order)
        {
            await bus.Publish(new OrderContract
            {
                order = order.ToDto(),
                eventType = EventType.Created
            });
        }

        public async Task SendOrderUpdatedMessageAsync(OrderEntity order)
        {
            await bus.Publish(new OrderContract
            {
                order = order.ToDto(),
                eventType = EventType.Updated
            });
        }

        public async Task SendOrderDeletedMessageAsync(OrderEntity order)
        {
            await bus.Publish(new OrderContract
            {
                order = order.ToDto(),
                eventType = EventType.Deleted
            });
        }
    }
}
