// Kafka producer client wrapper that encapsulates and manages the sending of messages to Kafka topics
// in a clean and reusable way
namespace InventoryService.Messaging.Clients;

using Confluent.Kafka;
using Contracts;

public class KafkaProducerClient : IKafkaProducerClient
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducerClient(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public async Task<DeliveryResult<string, string>> ProduceAsync(string topic, string key, string message)
    {
        var kafkaMessage = new Message<string, string> { Key = key, Value = message };

        return await _producer.ProduceAsync(topic, kafkaMessage);
    }

    public void Flush(TimeSpan timeout) => _producer.Flush(timeout);
    
    public void Dispose() => _producer.Dispose();
}