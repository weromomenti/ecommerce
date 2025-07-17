using InventoryService.Persistance.Dtos;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Persistance.Infrastructure
{
    public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {
        public DbSet<ProductEntity> Products { get; set; }
    }
}
