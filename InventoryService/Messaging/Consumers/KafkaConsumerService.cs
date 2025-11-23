// Kafka Consumer Service enables InventoryService to react asynchronously and continuously to inventory-related
// events published on Kafka.
namespace InventoryService.Messaging.Consumers;

using Configurations;
using Contracts;
using Microsoft.Extensions.Options;
using Models;
using System.Text.Json;

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly IKafkaConsumerClient _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageHandlerService _messageHandler;
    private readonly KafkaConsumerSettings _settings;

    public KafkaConsumerService( 
        IKafkaConsumerClient consumer,
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider,
        IOptions<KafkaConsumerSettings> options,
        IMessageHandlerService messageHandler)
    {
        _consumer = consumer;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _messageHandler = messageHandler;
        _settings = options.Value;
    }
    
    public async Task ConsumeInventoryEventMessagesAsync(CancellationToken token)
    {
        _consumer.Subscribe(_settings.Topic!);
        
        while (!token.IsCancellationRequested)
        {
            try
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
    }
}