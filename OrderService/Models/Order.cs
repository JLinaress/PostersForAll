// Using Object-Relational Mapping (ORM) to bridge between SQL tables and objects-oriented entities.

using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Order
{
    public int Id { get; set; }
    
    [StringLength(20)] // SKU max length 20 characters
    public string? ItemSku { get; set; }
    
    public int Quantity { get; set; }
    
    [StringLength(10)] // Status max length 10 characters
    public string? Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
}