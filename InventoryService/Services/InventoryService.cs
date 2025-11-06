// Business logic for managing inventory items.
// This class will implement methods to add, update, and retrieve inventory items.
namespace InventoryService.Services;

using Contracts;
using Data;
using Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class InventoryService : IInventoryService
{
    private readonly InventoryContext _context;
    private readonly IKafkaProducerService _kafkaProducerService;
    
    public InventoryService(InventoryContext context, IKafkaProducerService kafkaProducerService)
    {
        _context = context;
        _kafkaProducerService = kafkaProducerService;
    }
    
    public async Task<List<InventoryItem>> GetAllInventoryItemsAsync()
    {
        Console.WriteLine("Received GetAllInventoryItems request");
        
        return await _context.InventoryItems.ToListAsync();
    }
    
    public async Task<InventoryItem?> GetInventoryItemBySkuAsync(string sku)
    {
        return await _context.InventoryItems.FirstOrDefaultAsync(i => i.Sku!.ToLower() == sku.ToLower());
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
        
        // Optionally, publish an inventory update event to Kafka
        var inventoryEvent = JsonConvert.SerializeObject( new
        {
            item.Sku,
            item.QuantityAvailable,
            item.LastUpdated
        });
        
        // publish to event after inventory item is added or updated
        await _kafkaProducerService.ProduceInventoryEventMessageAsync(item.Sku!, inventoryEvent);
        
        return existingItem ?? item;
    }
}