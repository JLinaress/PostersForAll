// This InventoryContext class enables interaction with the Orders table through Entity Framework’s ORM capabilities
// Creates a portable, platform-agnostic SQL schema and EF Core integration for each microservice, ready for business logic and Kafka integration.

namespace InventoryService.Data;

using Microsoft.EntityFrameworkCore;
using Models;

public class InventoryContext : DbContext
{
    public InventoryContext(DbContextOptions<InventoryContext> options) : base(options)
    {
    }
    
    public DbSet<InventoryItem> InventoryItems { get; set; }
}