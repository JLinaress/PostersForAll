namespace InventoryService.Contracts;

using Confluent.Kafka;

public interface IKafkaProducerClient : IDisposable
{
    Task<DeliveryResult<string, string>> ProduceAsync(string topic, string key, string message);
    
    void Flush(TimeSpan timeout);
}