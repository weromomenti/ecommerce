using Ecommerce.Library.Dtos;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrustructure
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<OrderDto> Orders { get; set; }
    }
}
