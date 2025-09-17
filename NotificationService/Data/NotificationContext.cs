// This NotificationContext class enables interaction with the Orders table through Entity Framework’s ORM capabilities

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