// Business logic for managing inventory items.
// This class will implement methods to add, update, and retrieve inventory items.

namespace InventoryService.Services;

using Contracts;
using Data;
using Models;
using Microsoft.EntityFrameworkCore;

public class InventoryService : IInventoryService
{
    private readonly InventoryContext _context;
    
    public InventoryService(InventoryContext context)
    {
        _context = context;
    }
    
    public async Task<List<InventoryItem>> GetAllInventoryItemsAsync()
    {
        return await _context.InventoryItems.ToListAsync();
    }
    
    public async Task<InventoryItem?> GetInventoryItemBySkuAsync(string sku)
    {
        return await _context.InventoryItems.FirstOrDefaultAsync(i => i.Sku == sku);
    }
    
    public async Task<InventoryItem> AddOrUpdateInventoryItemAsync(InventoryItem item)
    {
        var existingItem = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Sku == item.Sku);
        if (existingItem != null)
        {
            existingItem.QuantityAvailable = item.QuantityAvailable;
            existingItem.LastUpdated = DateTime.UtcNow;
        }
        else
        {
            item.LastUpdated = DateTime.UtcNow;
            _context.InventoryItems.Add(item);
        }
        
        await _context.SaveChangesAsync();
        
        return existingItem ?? item;
    }
}