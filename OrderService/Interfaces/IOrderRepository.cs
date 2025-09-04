using OrderService.Entities;

namespace OrderService.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderEntity>> GetAllOrders();
        Task<OrderEntity?> GetOrderByIdAsync(int id);
        Task<OrderEntity> AddOrderAsync(OrderEntity order);
        Task<IEnumerable<OrderEntity>> GetOrdersByProductNameAsync(string productName);
        Task<bool> UpdateOrderAsync(OrderEntity order);
        Task<bool> DeleteOrderAsync(int id);
    }
}
