// Logic layer interface for notification management operations
namespace NotificationService.Contracts;

using Models;

public interface INotificationService
{
    Task CreateAndSendNotificationAsync(string message, string type, Guid userId);
    
    Task<List<Notification>> GetAllNotificationsAsync();
    
    Task<Notification?> GetNotificationByIdAsync(Guid id);
    
    Task<Notification> AddNotificationAsync(Notification notification);
}