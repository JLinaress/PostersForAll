// This OrdersContext class enables interaction with the Orders table through Entity Framework’s ORM capabilities

namespace OrderService.Data;

using Microsoft.EntityFrameworkCore;
using Models;

public class OrdersContext : DbContext
{
    public OrdersContext(DbContextOptions<OrdersContext> options) : base(options)
    {
    }
    
    public DbSet<Order> Orders { get; set; }
}