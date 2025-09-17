// This OrdersContext class enables interaction with the Orders table through Entity Framework’s ORM capabilities
// Creates a portable, platform-agnostic SQL schema and EF Core integration for each microservice, ready for business logic and Kafka integration.
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