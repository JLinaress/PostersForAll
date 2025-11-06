// Kafka Consumer Service enables InventoryService to react asynchronously and continuously to inventory-related
// events published on Kafka.
namespace InventoryService.Messaging.Consumers;

using Configurations;
using Contracts;
using Microsoft.Extensions.Options;

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly IKafkaConsumerClient _consumer;
    private readonly string _topic;
    private readonly IMessageHandlerService _messageHandler;

    public KafkaConsumerService(IOptions<KafkaConsumerSettings> kafkaSettings, 
        IKafkaConsumerClient consumer, 
        IMessageHandlerService messageHandler)
    {
        _topic = kafkaSettings.Value.Topic!;
        _consumer = consumer;
        _messageHandler = messageHandler;
        _consumer.Subscribe(_topic);
    }
    
    public async Task ConsumeInventoryEventMessagesAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(token);
                
                if (consumeResult != null)
                {
                    await _messageHandler.HandleMessageAsync(consumeResult.Message.Value);
                    // Process the consumed message
                    Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");

                    // Commit the offset after processing
                    _consumer.CommitAsync(consumeResult);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
            Console.WriteLine("Consumption cancelled.");
        }
        finally
        {
            _consumer.Close();
        }
    }
}