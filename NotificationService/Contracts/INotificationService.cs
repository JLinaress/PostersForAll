// Logic layer interface for notification management operations

namespace NotificationService.Contracts;

using Models;

public interface INotificationService
{
    Task SendNotificationAsync(string message, string type);
    
    Task<List<Notification>> GetAllNotificationsAsync();
    
    Task<Notification?> GetNotificationByIdAsync(int id);
    
    Task<Notification> AddNotificationAsync(Notification notification);
}