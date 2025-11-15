namespace NotificationService.Configurations;

public class KafkaConsumerSettings
{
    public string? BootstrapServers { get; set; }
    
    public string? Topic { get; set; }
}