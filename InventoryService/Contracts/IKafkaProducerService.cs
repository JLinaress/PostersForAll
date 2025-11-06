// interface abstracts the actual Kafka message sending logic, so the service can call
// this method whenever it needs to publish inventory-related events
// (like inventory reserved, inventory updated, or inventory failed events) to Kafka.
namespace InventoryService.Contracts;

public interface IKafkaProducerService
{
    public Task ProduceInventoryEventMessageAsync(string topic, string message);
}