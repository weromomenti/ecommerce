namespace InventoryService.Persistance.Dtos
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int AvailableStock { get; set; }
        public int ReservedStock { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
