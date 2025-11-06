// Business logic for inventory management
// This class will implement methods for adding, updating, and retrieving inventory items
namespace InventoryService.Controllers;

using Contracts;
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
    public async Task<IActionResult> GetAllInventoryItems()
    {
        var items = await _inventoryService.GetAllInventoryItemsAsync();
     
        return Ok(items);
    }

    [HttpGet("{sku}")]
    public async Task<IActionResult> GetInventoryItemBySku(string sku)
    {
        var item = await _inventoryService.GetInventoryItemBySkuAsync(sku);
        if (item == null)
            return NotFound();

        return Ok(item);
    }
    
    [HttpPost("addOrUpdate")]
    public async Task<IActionResult> AddOrUpdateInventoryItem([FromBody] InventoryItem item)
    {
        var updatedItem = await _inventoryService.AddOrUpdateInventoryItemAsync(item);
        
        return CreatedAtAction(nameof(GetInventoryItemBySku), new { sku = updatedItem.Sku }, updatedItem);
    }
}