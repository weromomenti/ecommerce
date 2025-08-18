using OrderService.Dtos;
using OrderService.Entities;

namespace OrderService.Messaging
{
    public sealed record OrderContract
    {
        public OrderDto order;

        public EventType eventType;
    }
}
