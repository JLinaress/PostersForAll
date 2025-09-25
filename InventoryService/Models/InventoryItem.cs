// Using Object-Relational Mapping (ORM) to bridge between SQL tables and objects-oriented entities.

using System.ComponentModel.DataAnnotations;

namespace InventoryService.Models;

public class InventoryItem
{
    public int Id { get; set; }
    
    [StringLength(20)] // SKU max length 20 characters
    public string? Sku { get; set; }
    
    public int QuantityAvailable { get; set; }
    
    public DateTime LastUpdated { get; set; }
}