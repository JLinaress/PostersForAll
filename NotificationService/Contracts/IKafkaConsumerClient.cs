namespace NotificationService.Contracts;

using Confluent.Kafka;

public interface IKafkaConsumerClient : IDisposable
{
    void Subscribe(string topic);
    
    void CommitAsync(ConsumeResult<string, string> consumeResult);
    
    ConsumeResult<string, string> Consume(CancellationToken cancellationToken);
    
    void Close();
}