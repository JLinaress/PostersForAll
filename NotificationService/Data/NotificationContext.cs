// This InventoryContext class enables interaction with the Orders table through Entity Framework’s ORM capabilities
// Creates a portable, platform-agnostic SQL schema and EF Core integration for each microservice, ready for business logic and Kafka integration.

namespace NotificationService.Data;

using Microsoft.EntityFrameworkCore;
using Models;

public class NotificationContext : DbContext
{
    public NotificationContext(DbContextOptions<NotificationContext> options) : base(options)
    {
    }
    
    public DbSet<Notification> Notifications { get; set; }
}