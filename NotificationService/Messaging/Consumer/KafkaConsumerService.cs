// Consumer service for processing notification event messages from Kafka
namespace NotificationService.Messaging.Consumer;

using Configurations;
using Confluent.Kafka;
using Contracts;
using Microsoft.Extensions.Options;
using Models;
using System.Text.Json;

public class KafkaConsumerService : IKafkaConsumerService, IDisposable
{
    private readonly IConsumer<string,string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaConsumerSettings _settings;
    // private readonly IMessageHandlerService _messageHandler;
    private bool _disposed = false;
    
    public KafkaConsumerService(
        IOptions<KafkaConsumerSettings> options,
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider//,
        // IMessageHandlerService messageHandler)
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _settings = options.Value;
        // _messageHandler = messageHandler;
        
        _consumer = new ConsumerBuilder<string, string>(new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        }).SetErrorHandler((_, e) => _logger.LogError($"Kafka Consumer error: {e.Reason}"))
            .Build();
        
        // startup logging for dev / demo purposes
        _logger.LogInformation($"✅ Kafka CONSUMER initialized in NotificationService {_settings.BootstrapServers}");
    }

    public async Task ConsumeNotificationEventMessagesAsync(CancellationToken token)
    {
        _consumer.Subscribe(_settings.Topic!);

        try
        {
            while (!token.IsCancellationRequested)
            {
                var result = _consumer.Consume(token);

                if (result != null)
                {
                    var payload = JsonSerializer.Deserialize<NotificationEventPayload>(result.Message.Value);

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
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        await notificationService.AddNotificationAsync(notification);
                    }

                    _consumer.Commit(result);
                    _logger.LogInformation($"Processed notification event: {payload.NotificationId}");
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            // Graceful shutdown
            _logger.LogInformation($"Kafka consumer is shutting down gracefully: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error consuming notification event messages: {ex.Message}");
        }
        finally
        {
            _consumer.Close();
        }
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            _consumer.Dispose();
            _disposed = true;
        }
    }
}