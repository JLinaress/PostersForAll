namespace InventoryService.Models;

public class InventoryUpdateEvent
{
    public Guid ProductId { get; set; }
    
    public int QuantityChange { get; set; }
    
    public string EventType { get; set; } = string.Empty;
    
    public DateTime EventTimestamp { get; set; }
    
    public  Guid OrderId { get; set; }
    
    public Guid OrderItemId { get; set; }
}