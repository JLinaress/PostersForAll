// Implementing IKafkaConsumerClient as a wrapper around consumer
namespace InventoryService.Messaging.Clients;

using Confluent.Kafka;
using Contracts;

public class KafkaConsumerClient : IKafkaConsumerClient
{
    private readonly IConsumer<string, string> _consumer;
    
    public KafkaConsumerClient(IConsumer<string, string> consumer)
    {
        _consumer = consumer;
    }

    public void Subscribe(string topic) => _consumer.Subscribe(topic);
    
    public void CommitAsync(ConsumeResult<string, string> consumeResult) => 
        _consumer.Commit(consumeResult);
    
    public ConsumeResult<string, string> Consume(CancellationToken cancellationToken) => 
        _consumer.Consume(cancellationToken);
    
    public void Close() => _consumer.Close();
    
    public void Dispose() => _consumer.Dispose();
}