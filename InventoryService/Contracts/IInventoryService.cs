// Logic layer interface for inventory management operations
namespace InventoryService.Contracts;

using Models;

public interface IInventoryService
{
    Task<List<InventoryItem>> GetInventoryItemsAsync();
    
    Task<InventoryItem?> GetInventoryItemByProductIdAsync(Guid product);
    
    Task<InventoryUpdateResult> AddOrUpdateInventoryItemAsync(InventoryUpdateEvent update);
}