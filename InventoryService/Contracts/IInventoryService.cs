// Logic layer interface for inventory management operations

using InventoryService.Models;

namespace InventoryService.Contracts;

public interface IInventoryService
{
    Task<List<InventoryItem>> GetAllInventoryItemsAsync();
    
    Task<InventoryItem?> GetInventoryItemBySkuAsync(string sku);
    
    Task<InventoryItem> AddOrUpdateInventoryItemAsync(InventoryItem item);
}