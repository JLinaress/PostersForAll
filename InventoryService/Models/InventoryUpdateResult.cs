namespace InventoryService.Models;

public class InventoryUpdateResult
{
    public InventoryItem? Item { get; set; }
    
    public InventoryChangeResult Result { get; set; }
}