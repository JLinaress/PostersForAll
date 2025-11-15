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

    public KafkaProducerService(IOptions<KafkaConsumerSettings> settings, IProducer<string, string> producer)
    {
        _topic = settings.Value.Topic ?? throw new ArgumentNullException(nameof(settings.Value.Topic));
        _producer = producer;
    }

    public async Task ProduceNotificationEventAsync(string key, string value)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = value
        };

        await _producer.ProduceAsync(_topic, kafkaMessage);
        // Optionally handle delivery reports or errors here later on
    }
}