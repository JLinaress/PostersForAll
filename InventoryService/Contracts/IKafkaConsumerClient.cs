// Kafka Consumer interface defines the essential operations required by the InventoryService to consume
// and process Kafka messages reliably and cleanly 
namespace InventoryService.Contracts;

using Confluent.Kafka;

public interface IKafkaConsumerClient
{
    void Subscribe(string topic);
    
    void CommitAsync(ConsumeResult<string, string> consumeResult);
    
    ConsumeResult<string, string> Consume(CancellationToken cancellationToken);
    
    void Close();
}