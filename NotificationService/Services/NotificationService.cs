// Business logic for sending notifications
// This class will implement methods to send notifications and retrieve notification history

namespace NotificationService.Services;

using Microsoft.EntityFrameworkCore;
using Contracts;
using Data;
using Models;

public class NotificationService : INotificationService
{
    private readonly NotificationContext _context;

    public NotificationService(NotificationContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetAllNotificationsAsync()
    {
        return await _context.Notifications.ToListAsync();
    }
    
    public async Task<Notification?> GetNotificationByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }
    
    public async Task<Notification> AddNotificationAsync(Notification notification)
    {
        notification.CreatedAt = DateTime.UtcNow;
        
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        
        return notification;
    }
    
    public async Task SendNotificationAsync(string message, string type)
    {
        // Simulate sending notification (e.g., via email, SMS, push, etc.)
        Console.WriteLine($"Sending {type} notification: {message}");
        
        // Log the notification in the database
        var notification = new Notification
        {
            Message = message,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}