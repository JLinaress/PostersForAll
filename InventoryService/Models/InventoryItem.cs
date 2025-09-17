// Using Object-Relational Mapping (ORM) to bridge between SQL tables and objects-oriented entities.

namespace InventoryService.Models;

public class InventoryItem
{
    public int Id { get; set; }
    
    public string Sku { get; set; }
    
    public int QuantityAvailable { get; set; }
    
    public DateTime LastUpdated { get; set; }
}