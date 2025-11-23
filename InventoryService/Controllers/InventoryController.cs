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
    
    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryItems()
    {
        var items = await _inventoryService.GetInventoryItemsAsync();
     
        return Ok(items);
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetInventoryItemByProduct(Guid productId)
    {
        var item = await _inventoryService.GetInventoryItemByProductIdAsync(productId);
        if (item == null)
            return NotFound();

        return Ok(item);
    }
    
    [HttpPost("addOrUpdate")]
    public async Task<IActionResult> AddOrUpdateInventoryItem([FromBody] InventoryItem item)
    {
        var updatedItem = new InventoryUpdateEvent
        {
            ProductId = item.ProductId!,
            QuantityChange = item.QuantityAvailable,
            EventTimestamp = DateTime.UtcNow,
            EventType = "Update"
        };
        
        var updateResult = await _inventoryService.AddOrUpdateInventoryItemAsync(updatedItem);
        
        if (updateResult.Item == null || updateResult.Result == InventoryChangeResult.Failed)
            return BadRequest($"Failed to add or update inventory item");
        
        if (updateResult.Result == InventoryChangeResult.Created)
            return CreatedAtAction(nameof(GetInventoryItemByProduct), new { productId = updateResult.Item.ProductId }, updateResult.Item);
        
        // For updates, return 200 OK with the updated item
        return Ok(updateResult.Item);
    }

    [HttpPost("checkOrder")]
    public async Task<IActionResult> CheckOrderInventory([FromBody] OrderDto order)
    {
        // check stock availability or update inventory based on order
        foreach (var orderItem in order.Items!)
        {
            // check inventory for each order item
            var inventoryItem = await _inventoryService.GetInventoryItemByProductIdAsync(orderItem.ProductId);
            
            if (inventoryItem == null || inventoryItem.QuantityAvailable < orderItem.Quantity)
            {
                return Ok( new InventoryCheckResult
                {
                    Success = false,
                    Message =  $"Unable to process request because insufficient stock for Product ID: {orderItem.ProductId}"
                });
            }
        }

        return Ok( new InventoryCheckResult
        {
            Success = true,
            Message = "All items are in stock."
        });
    }
}