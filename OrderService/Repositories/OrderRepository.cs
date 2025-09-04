using Microsoft.EntityFrameworkCore;
using OrderService.Entities;
using OrderService.Infrustructure;
using OrderService.Interfaces;

namespace OrderService.Repositories
{
    public class OrderRepository(
        OrderDbContext context,
        ILogger<OrderRepository> logger) : IOrderRepository
    {
        public async Task<IEnumerable<OrderEntity>> GetAllOrders()
        {
            logger.LogInformation("Retrieving all orders from the database...");
            var orderDtos = await context.Orders.ToArrayAsync();
            return orderDtos.Select(o => o.ToEntity());
        }
        public async Task<OrderEntity?> GetOrderByIdAsync(int id)
        {
            var orderDto = await context.Orders.FindAsync(id);


            return orderDto?.ToEntity();
        }

        public async Task<OrderEntity> AddOrderAsync(OrderEntity order)
        {
            logger.LogInformation("Adding order Product Name: {productName}, Quantity: {quantity}", 
                order.ProductName, order.Quantity);

            var orderDto = order.ToDto();

            await context.Orders.AddAsync(orderDto);
            await context.SaveChangesAsync();

            return orderDto.ToEntity();
        }

        public async Task<IEnumerable<OrderEntity>> GetOrdersByProductNameAsync(string productName)
        {
            var orderDtos = await context.Orders
                .Where(o => o.ProductName == productName)
                .ToListAsync();

            return orderDtos.Select(o => o.ToEntity());
        }

        public async Task<bool> UpdateOrderAsync(OrderEntity newOrder)
        {
            var orderDto = newOrder.ToDto();
            context.Orders.Update(orderDto);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var orderDto = await context.Orders.FindAsync(id);
            if (orderDto == null) return false;

            context.Orders.Remove(orderDto);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
