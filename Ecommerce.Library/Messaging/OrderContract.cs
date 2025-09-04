using Ecommerce.Library.Dtos;

namespace Ecommerce.Library.Messaging
{
    public record OrderContract
    {
        public OrderContract() { }
        
        public OrderContract(OrderDto order, EventType eventType)
        {
            this.order = order;
            this.eventType = eventType;
        }

        public OrderDto order { get; set; } = null!;
        public EventType eventType { get; set; }
    }
}
