// core building block necessary whenever your service must publish messages to Kafka
namespace InventoryService.Services;

using Confluent.Kafka;
using Configurations;
using Contracts;
using Microsoft.Extensions.Options;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IKafkaProducerClient _producer;
    private readonly string _topic;
    
    public KafkaProducerService(IOptions<KafkaConsumerSettings> kafkaSettings, IKafkaProducerClient producer)
    {
        var settings = kafkaSettings.Value;
        _topic = settings.Topic!;
        _producer = producer;
    }

    public async Task ProduceInventoryEventMessageAsync(string key, string message)
    {
        try
        {
            var deliveryResult = await _producer.ProduceAsync(_topic, key, message);

            // Log the inventory event message delivery
            Console.WriteLine($"Inventory event message delivered to {deliveryResult.TopicPartitionOffset}");
        }
        catch (KafkaException kex)
        {
            // handle Kafka-specific exceptions
            Console.WriteLine($"Kafka error producing message: {kex.Error.Reason}");
        }
        catch (Exception ex)
        {
            // handle unexpected errors for inventory event message production
            Console.WriteLine($"Inventory delivery failed: {ex.Message}");
        }
    }
}