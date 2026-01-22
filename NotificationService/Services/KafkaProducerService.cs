// Kafka producer class that created notification
namespace NotificationService.Services;

using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Configurations;
using Contracts;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducerService(IOptions<KafkaConsumerSettings> settings)
    {
        var setting = settings.Value;
        _topic = setting.Topic!;
        
        var config = new ProducerConfig
        {
            BootstrapServers = settings.Value.BootstrapServers,
            ClientId = "notification-service-producer"
        };
        
        _producer = new ProducerBuilder<string, string>(config).Build();
        
        // startup logging for dev / demo purposes
        Console.WriteLine($"✅ Kafka PRODUCER initialized in NotificationService {config.BootstrapServers}");
    }

    public async Task ProduceNotificationEventAsync(string key, string value)
    {
        try
        {

            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = value
            };

            var deliveryResult = await _producer.ProduceAsync(_topic, kafkaMessage);

            // Log the delivery result to console, can come back later and remove it when not needed
            Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
        }
        catch (KafkaException ex)
        {
            // Handle Kafka-specific exception
            Console.WriteLine($"Kafka error producing message: {ex.Error.Reason}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delivery failed: {ex.Message}");
        }
    }
}