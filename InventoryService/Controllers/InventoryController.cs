// Business logic for inventory management
// This class will implement methods for adding, updating, and retrieving inventory items
namespace InventoryService.Controllers;

using Contracts;
using Dtos;
using Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService; // service layer dependency
    private readonly ILogger<InventoryController> _logger; // logging dependency
    
    public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryItems()
    {
        _logger.LogInformation("Inventory items retrieval requested");
        
        var items = await _inventoryService.GetInventoryItemsAsync();
     
        _logger.LogInformation("Inventory items retrieved: {ItemCount}", items.Count);
        return Ok(items);
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetInventoryItemByProduct(Guid productId)
    {
        _logger.LogInformation("Inventory item retrieval requested");
        var item = await _inventoryService.GetInventoryItemByProductIdAsync(productId);
        if (item == null)
        {
            _logger.LogWarning("Inventory item not found for Product ID: {ProductId}", productId);
            return NotFound();
        }

        _logger.LogInformation("Inventory item retrieved for Product ID: {ProductId}", productId);
        return Ok(item);
    }
    
    [HttpPost("addOrUpdate")]
    public async Task<IActionResult> AddOrUpdateInventoryItem([FromBody] InventoryItem item)
    {
        _logger.LogInformation("Add or update inventory item requested for Product ID: {ProductId}", item.ProductId);
        var updatedItem = new InventoryUpdateEvent
        {
            ProductId = item.ProductId!,
            QuantityChange = item.QuantityAvailable,
            EventTimestamp = DateTime.UtcNow,
            EventType = "Update"
        };
        
        var updateResult = await _inventoryService.AddOrUpdateInventoryItemAsync(updatedItem);

        if (updateResult.Item == null || updateResult.Result == InventoryChangeResult.Failed)
        {
            _logger.LogError("Failed to add or update inventory item for Product ID: {ProductId}", item.ProductId);
            return BadRequest($"Failed to add or update inventory item");
        }

        if (updateResult.Result == InventoryChangeResult.Created)
        {
            _logger.LogInformation("Inventory item added for Product ID: {ProductId}", item.ProductId);
            return CreatedAtAction(nameof(GetInventoryItemByProduct), new { productId = updateResult.Item.ProductId }, updateResult.Item);
        }
        
        _logger.LogInformation("Inventory item updated for Product ID: {ProductId}", item.ProductId);
        // For updates, return 200 OK with the updated item
        return Ok(updateResult.Item);
    }

    [HttpPost("checkOrder")]
    public async Task<IActionResult> CheckOrderInventory([FromBody] OrderDto order)
    {
        // check stock availability or update inventory based on order
        foreach (var orderItem in order.Items!)
        {
            _logger.LogInformation("Checking order item for Product ID: {ProductId}", orderItem.ProductId);
            // check inventory for each order item
            var inventoryItem = await _inventoryService.GetInventoryItemByProductIdAsync(orderItem.ProductId);
            
            if (inventoryItem == null || inventoryItem.QuantityAvailable < orderItem.Quantity)
            {
                _logger.LogWarning("Insufficient stock for Product ID: {ProductId}", orderItem.ProductId);
                return Ok( new InventoryCheckResult
                {
                    Success = false,
                    Message =  $"Unable to process request because insufficient stock for Product ID: {orderItem.ProductId}"
                });
            }
        }

        _logger.LogInformation("All items in stock for Order ID: {ProductId}", order.OrderId);
        return Ok( new InventoryCheckResult
        {
            Success = true,
            Message = "All items are in stock."
        });
    }
}