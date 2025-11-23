namespace InventoryService.Dtos;

public class OrderDto
{
    // TODO: Use it to check for unique orders and prevent duplicates. Update to GUID later.
    // It'll break schema, need to run migration command.
    public string? OrderId { get; set; }
    
    public List<OrderItemDto>? Items { get; set; }
}