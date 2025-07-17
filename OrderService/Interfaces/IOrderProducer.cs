using OrderService.Entities;

namespace OrderService.Interfaces;

public interface IOrderProducer
{
    Task SendOrderCreatedMessageAsync(OrderEntity order);

    Task SendOrderUpdatedMessageAsync(OrderEntity order);

    Task SendOrderDeletedMessageAsync(OrderEntity order);
}