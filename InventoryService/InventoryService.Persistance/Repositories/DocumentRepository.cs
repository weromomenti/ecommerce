using AutoMapper;
using InventoryService.Business.Entities;
using InventoryService.Business.Interfaces;
using InventoryService.Persistance.Documents;
using InventoryService.Persistance.Infrastructure;
using MongoDB.Driver;

namespace InventoryService.Persistance.Repositories
{
    internal class DocumentRepository(
        MongoDbContext mongoDbContext,
        IMapper mapper) : IDocumentRepository
    {
        public async Task<IEnumerable<string>> GetAllDocumentNamesAsync()
        {
            throw new NotSupportedException("Decided not to do this lol");
        }

        public async Task<InventoryMovementDto?> GetDocumentByOrderAsync(int orderId)
        {
            var document = await mongoDbContext.Orders
                .Find(d => d.OrderId == orderId)
                .SortByDescending(d => d.UpdatedAt)
                .FirstOrDefaultAsync();
            
            return document != null ? mapper.Map<InventoryMovementDto>(document) : null;
        }

        public async Task<IEnumerable<InventoryMovementDto>> GetDocumentsByProductAsync(string productName)
        {
            var documents = await mongoDbContext.Orders
                .Find(d => d.ProductName == productName)
                .ToListAsync();
            
            return mapper.Map<IEnumerable<InventoryMovementDto>>(documents);
        }

        public async Task<IEnumerable<InventoryMovementDto>> GetDocumentByFilerAsync(Func<InventoryMovementDto, bool> filter)
        {
            // Note: This approach loads all documents into memory first, then applies the filter
            // For large datasets, consider implementing server-side filtering
            var allDocuments = await mongoDbContext.Orders.Find(_ => true).ToListAsync();
            var allEntities = mapper.Map<IEnumerable<InventoryMovementDto>>(allDocuments);
            
            return allEntities.Where(filter);
        }

        public async Task AddDocumentAsync(InventoryMovementDto dto)
        {
            var document = mapper.Map<OrderDocumentEntity>(dto);
            await mongoDbContext.Orders.InsertOneAsync(document);
        }

        public async Task UpdateDocumentAsync(InventoryMovementDto dto)
        {
            var document = mapper.Map<OrderDocumentEntity>(dto);
            await mongoDbContext.Orders.UpdateOneAsync(
                d => d.OrderId == document.OrderId,
                Builders<OrderDocumentEntity>.Update
                    .Set(d => d.ProductName, dto.ProductName)
                    .Set(d => d.Quantity, dto.Quantity));
        }

        public Task<bool> DeleteDocumentAsync(string documentName)
        {
            throw new NotImplementedException();
        }
    }
}
