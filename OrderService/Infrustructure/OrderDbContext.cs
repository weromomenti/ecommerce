using Microsoft.EntityFrameworkCore;
using OrderService.Dtos;

namespace OrderService.Infrustructure
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<OrderDto> Orders { get; set; }
    }
}
