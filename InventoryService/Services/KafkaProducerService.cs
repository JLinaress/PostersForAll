// core building block necessary whenever your service must publish messages to Kafka
namespace InventoryService.Services;

using Confluent.Kafka;
using Configurations;
using Contracts;
using Microsoft.Extensions.Options;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    
    public KafkaProducerService(IOptions<KafkaConsumerSettings> kafkaSettings)
    {
        _topic = kafkaSettings.Value.Topic!;
        
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            ClientId = "inventory-service-producer"
        };
        
        _producer = new ProducerBuilder<string, string>(config).Build();
        
        // startup logging for dev / demo purposes
        Console.WriteLine($"✅ Kafka PRODUCER initialized in InventoryService {config.BootstrapServers}");
    }

    public async Task ProduceInventoryEventMessageAsync(string key, string message)
    {
        try
        {
            var kafkaMessage = new Message<string, string> { Key = key, Value = message };

            var deliveryResult = await _producer.ProduceAsync(_topic, kafkaMessage);

            // Log the delivery result to console, can come back later and remove it when not needed
            Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");

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