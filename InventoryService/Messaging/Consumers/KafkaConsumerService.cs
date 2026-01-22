// Kafka Consumer Service enables InventoryService to react asynchronously and continuously to inventory-related
// events published on Kafka.
namespace InventoryService.Messaging.Consumers;

using Configurations;
using Confluent.Kafka;
using Contracts;
using Microsoft.Extensions.Options;
using Models;
using System.Text.Json;

public class KafkaConsumerService : IKafkaConsumerService, IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageHandlerService _messageHandler;
    private readonly KafkaConsumerSettings _settings;
    private bool _disposed = false;

    public KafkaConsumerService( 
        IOptions<KafkaConsumerSettings> options,
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider,
        IMessageHandlerService messageHandler)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _messageHandler = messageHandler;
        _settings = options.Value;
        
        _consumer = new ConsumerBuilder<string, string>(new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        }).SetErrorHandler((_, e) => _logger.LogError($"Kafka Consumer error: {e.Reason}"))
            .Build();
        
        // startup logging for dev / demo purposes
        _logger.LogInformation($"✅ Kafka CONSUMER initialized in InventoryService {_settings.BootstrapServers}");
    }
    
    public async Task ConsumeInventoryEventMessagesAsync(CancellationToken token)
    {
        // Direct confluent call
        _consumer.Subscribe(_settings.Topic!);

        try
        { 
            while (!token.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(token);

                if (consumeResult != null)
                {
                    await _messageHandler.HandleMessageAsync(consumeResult.Message.Value);

                    // Process the consumed message
                    var update = JsonSerializer.Deserialize<InventoryUpdateEvent>(consumeResult.Message.Value);

                    // use a scoped service to handle the DB inventory update
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

                        if (update != null)
                        {
                            var item = await inventoryService.GetInventoryItemByProductIdAsync(update.ProductId);
                            if (item != null)
                            {
                                item.QuantityAvailable += update.QuantityChange;
                                await inventoryService.AddOrUpdateInventoryItemAsync(update);

                                _logger.LogInformation($"Inventory updated for SKU: {update.ProductId}, New Quantity: {item.QuantityAvailable}");
                            }
                            else
                            {
                                _logger.LogWarning($"Inventory item with Product: {update.ProductId} not found.");
                            }
                        }
                    }

                    // Commit the message offset after successful processing
                    _consumer.Commit(consumeResult);
                    _logger.LogInformation($"Processed inventory event: {update?.ProductId}");
                }
            }
        }
        catch (OperationCanceledException oex)
        {
            // Handle cancellation
            Console.WriteLine(oex.Message, "Error consuming kafka message.");
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