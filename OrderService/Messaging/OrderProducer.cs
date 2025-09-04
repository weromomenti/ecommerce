using OrderService.Interfaces;
using OrderService.Entities;
using MassTransit;
using OrderService.Infrustructure;
using Ecommerce.Library.Messaging;

namespace OrderService.Messaging
{
    public class OrderProducer(ISendEndpointProvider endpointProvider) : IOrderProducer
    {
        private const string QueueName = "order-queue";

        private async Task<ISendEndpoint> GetQueueAsync()
            => await endpointProvider.GetSendEndpoint(new Uri($"queue:{QueueName}"));

        public async Task SendOrderCreatedMessageAsync(OrderEntity order)
        {
            var endpoint = await GetQueueAsync();
            await endpoint.Send(new OrderContract
            {
                order = order.ToDto(),
                eventType = EventType.Created
            });
        }

        public async Task SendOrderUpdatedMessageAsync(OrderEntity order)
        {
            var endpoint = await GetQueueAsync();
            await endpoint.Send(new OrderContract
            {
                order = order.ToDto(),
                eventType = EventType.Updated
            });
        }

        public async Task SendOrderDeletedMessageAsync(OrderEntity order)
        {
            var endpoint = await GetQueueAsync();
            await endpoint.Send(new OrderContract
            {
                order = order.ToDto(),
                eventType = EventType.Deleted
            });
        }
    }
}
