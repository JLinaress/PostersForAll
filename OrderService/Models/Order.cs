// Using Object-Relational Mapping (ORM) to bridge between SQL tables and objects-oriented entities.

namespace OrderService.Models;

public class Order
{
    public int Id { get; set; }
    
    public string ItemSku { get; set; }
    
    public int Quantity { get; set; }
    
    public string Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
}