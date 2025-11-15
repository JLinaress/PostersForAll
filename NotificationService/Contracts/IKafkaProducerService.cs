namespace NotificationService.Contracts;

public interface IKafkaProducerService
{
    Task ProduceNotificationEventAsync(string key, string message);
}