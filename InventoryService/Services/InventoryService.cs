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
    
    public async Task<List<InventoryItem>> GetInventoryItemsAsync()
    {
        Console.WriteLine("Received GetAllInventoryItems request");
        
        return await _context.InventoryItems.ToListAsync();
    }
    
    public async Task<InventoryItem?> GetInventoryItemByProductIdAsync(Guid product)
    {
        return await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId! == product);
    }
    
    public async Task<InventoryUpdateResult> AddOrUpdateInventoryItemAsync(InventoryUpdateEvent update)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == update.ProductId);
        
        InventoryChangeResult result;
        if (item == null)
        {
            // create new inventory item if it doesn't exist
            item = new InventoryItem
            {
                ProductId = update.ProductId,
                QuantityAvailable = update.EventType == "Increase" ? update.QuantityChange : -update.QuantityChange,
                LastUpdated = update.EventTimestamp
            };
            
            _context.InventoryItems.Add(item);

            result = InventoryChangeResult.Created;
        }
        else
        {
            // update existing inventory item
            if (update.EventType == "Increase")
                item.QuantityAvailable += update.QuantityChange;
            if (update.EventType == "Decrease")
                item.QuantityAvailable -= update.QuantityChange;

            item.LastUpdated = update.EventTimestamp;

            result = InventoryChangeResult.Updated;
        }

        // save changes to the database
        await _context.SaveChangesAsync();
        
        // Optionally, publish an inventory update event to Kafka
        var inventoryEvent = new InventoryUpdateEvent
        {
            ProductId = update.ProductId,
            QuantityChange = update.QuantityChange,
            EventType = update.EventType,
            EventTimestamp = update.EventTimestamp,
            OrderId = update.OrderId,
            OrderItemId = update.OrderItemId
        };
        
        var serializedInventoryEvent = JsonConvert.SerializeObject(inventoryEvent);

        try
        {
            // publish to event after inventory item is added or updated    
            await _kafkaProducerService.ProduceInventoryEventMessageAsync(update.ProductId!.ToString(), serializedInventoryEvent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was an error publishing the inventory update event message to Kafka: {ex.Message}");
            throw;
        }
        
        return new InventoryUpdateResult
        {
            Item =  item,
            Result = result
        };
    }
}