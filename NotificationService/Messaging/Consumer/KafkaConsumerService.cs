// Consumer service for processing notification event messages from Kafka
namespace NotificationService.Messaging.Consumer;

using System.Text.Json;
using Microsoft.Extensions.Options;
using Configurations;
using Contracts;
using Models;

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly IKafkaConsumerClient _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaConsumerSettings _settings;
    
    public KafkaConsumerService(
        IKafkaConsumerClient consumer,
        IOptions<KafkaConsumerSettings> settings,
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider)
    {
        _consumer = consumer;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
    }

    public async Task ConsumeNotificationEventMessagesAsync(CancellationToken token)
    {
        _consumer.Subscribe(_settings.Topic!);

        while (!token.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(token);

                if (result != null)
                {
                    var payload =
                        JsonSerializer.Deserialize<NotificationEventPayload>(result.Message.Value);

                    // Map payload to notification entity and save using scoped service
                    var notification = new Notification
                    {
                        Id = payload!.NotificationId,
                        Message = payload.NotificationMessage!,
                        Type = payload.NotificationType!,
                        CreatedAt = DateTime.UtcNow
                    };

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService =
                            scope.ServiceProvider.GetRequiredService<INotificationService>();

                        await notificationService.AddNotificationAsync(notification);
                    }
                    
                    _consumer.CommitAsync(result);
                    _logger.LogInformation($"Processed notification event: {payload.NotificationId}");
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
                _logger.LogInformation("Kafka consumer is shutting down.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured in Kafka consumer loop");
            }
        }
        
        _consumer.Close();
    }
    
    public void Dispose()
    {
        _consumer.Dispose();
        _logger.LogInformation("Kafka consumer disposed");
    }
}