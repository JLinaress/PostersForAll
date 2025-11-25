namespace NotificationService.Contracts;

public interface IKafkaConsumerService
{
    Task ConsumeNotificationEventMessagesAsync(CancellationToken token);
}