using InventoryService.Business.Entities;

namespace InventoryService.Business.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<string>> GetAllDocumentNamesAsync();
        Task<IEnumerable<InventoryMovementDto>> GetDocumentsByProductAsync(string productName);
        Task<InventoryMovementDto?> GetDocumentByOrderAsync(int orderId);
        Task<IEnumerable<InventoryMovementDto>> GetDocumentByFilerAsync(Func<InventoryMovementDto, bool> filer);
        Task AddDocumentAsync(InventoryMovementDto orderDocument);
        Task UpdateDocumentAsync(InventoryMovementDto orderDocument);
        Task<bool> DeleteDocumentAsync(string documentName);
    }
}
