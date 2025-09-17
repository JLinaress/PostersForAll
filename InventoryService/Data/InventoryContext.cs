// This InventoryContext class enables interaction with the Orders table through Entity Framework’s ORM capabilities

namespace InventoryService.Data;

using Microsoft.EntityFrameworkCore;
using Models;

public class InventoryContext : DbContext
{
    public InventoryContext(DbContextOptions<InventoryContext> options)
    {
    }
    
    public DbSet<InventoryItem> InventoryItems { get; set; }
}