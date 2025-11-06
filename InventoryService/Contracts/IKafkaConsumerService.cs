// Kafka consumer service interface that focuses on consuming inventory-related Kafka event messages asynchronously.
namespace InventoryService.Contracts;

public interface IKafkaConsumerService
{
    Task ConsumeInventoryEventMessagesAsync(CancellationToken token);
}