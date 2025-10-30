// This service will handle producing messages to Kafka topics.
// It will include methods to send messages related to order events, such as order creation and updates

using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OrderService.Configuration;
using OrderService.Contracts;

namespace OrderService.Services;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducerService(IOptions<KafkaSettings> kafkaSettings)
    {
        var settings = kafkaSettings.Value;
        _topic = settings.Topic;
        
        var config = new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers, // Kafka broker address
            ClientId = "order-service-producer"
        };
        
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    
    public async Task ProduceOrderEventMessageAsync(string key, string message)
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
            // Handle Kafka-specific exception
            Console.WriteLine($"Kafka error producing message: {kex.Error.Reason}");
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            Console.WriteLine($"Delivery failed: {ex.Message}");
        }
    }
}