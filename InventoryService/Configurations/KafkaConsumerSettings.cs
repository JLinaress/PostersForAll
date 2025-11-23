namespace InventoryService.Configurations;

public class KafkaConsumerSettings
{
    public string? AutoOffsetReset { get; set; }
    
    public string? BootstrapServers { get; set; }
    
    public string? GroupId { get; set; }
    
    public string? Topic { get; set; }
}