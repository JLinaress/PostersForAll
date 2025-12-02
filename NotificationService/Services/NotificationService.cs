// Business logic for sending notifications
// This class will implement methods to send notifications and retrieve notification history
namespace NotificationService.Services;

using Contracts;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Prometheus;
using System.Text.Json;

public class NotificationService : INotificationService
{
    private readonly Counter NotificationCreatedCounter = Metrics.CreateCounter("notification_created_total", "Total number of notifications created");
    private readonly NotificationContext _context;
    private readonly IKafkaProducerService _kafkaProducer;

    public NotificationService(NotificationContext context, IKafkaProducerService kafkaProducer)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<List<Notification>> GetAllNotificationsAsync() =>
        await _context.Notifications.ToListAsync();

    public async Task<Notification?> GetNotificationByIdAsync(Guid id) =>
        await _context.Notifications.FindAsync(id);

    public async Task<Notification> AddNotificationAsync(Notification notification)
    {
        notification.CreatedAt = notification.CreatedAt == default 
            ? DateTime.UtcNow
            : notification.CreatedAt;

        // Check for existence first
        var exists = await _context.Notifications.AnyAsync(n => n.Id == notification.Id);
        if (exists)
        {
            // Option 1: Throw, caller should handle this
            throw new InvalidOperationException("A notification with this Id already exists.");
            // Option 2: Return null or meaningful error object
            // return null;
        }
        _context.Notifications.Add(notification);
        
        try
        {
            await _context.SaveChangesAsync();
            
            // Increment Prometheus counter
            NotificationCreatedCounter.Inc();
        }
        // Revisit and clean up. No need to throw exception here 
        catch (DbUpdateException ex) when ((ex.InnerException as Microsoft.Data.Sqlite.SqliteException)?.SqliteErrorCode == 19)
        {
            // Handle UNIQUE constraint violation (should not reach here with the pre-check)
            throw new InvalidOperationException("Duplicate Notification Id violation.", ex);
        }

        return notification;
    }

    public async Task CreateAndSendNotificationAsync(string message, string type, Guid userId)
    {
        // Log the notification in the database
        var notification = new Notification
        {
            Message = message,
            Type = type,
            Id = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        // save notification 
        var saveNotification = await AddNotificationAsync(notification);

        // Only publish event if save 
        if (saveNotification != null)
        {
            // Create event payload
            var payload = new NotificationEventPayload
            {
                NotificationId = notification.Id,
                NotificationMessage = notification.Message,
                NotificationType = notification.Type,
                NotificationCreatedAt = notification.CreatedAt
            };

            // Publish event to kafka
            await _kafkaProducer.ProduceNotificationEventAsync(
                notification.Id.ToString(),
                JsonSerializer.Serialize(payload));
        }
        else
        {
            // Handle failure to save notification, e.g., throw or log warning
            throw new Exception("Failed to save notification to the database.");
        }
    }
}