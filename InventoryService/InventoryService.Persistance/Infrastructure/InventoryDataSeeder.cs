using InventoryService.Persistance.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Persistance.Infrastructure
{
    public class InventoryDataSeeder
    {
        public static void Seed(InventoryDbContext context)
        {
            if (context.Products.Any())
            {
                return; // Data was already seeded
            }

            var products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    // Adjust property names/values as needed.
                    ProductName = "Keyboard",
                    AvailableStock = 100,
                    ReservedStock = 0,
                    Price = 9.99m,
                    LastUpdated = DateTime.UtcNow
                },
                new ProductEntity
                {
                    ProductName = "Mouse",
                    AvailableStock = 200,
                    ReservedStock = 0,
                    Price = 19.99m,
                    LastUpdated = DateTime.UtcNow
                },
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
