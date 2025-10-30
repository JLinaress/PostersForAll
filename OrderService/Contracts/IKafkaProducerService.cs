namespace OrderService.Contracts;

public interface IKafkaProducerService
{
    Task ProduceOrderEventMessageAsync(string topic, string message);
}