// Using Object-Relational Mapping (ORM) to bridge between SQL tables and objects-oriented entities.
// DTO (Data Transfer Object) for transferring notification event data.
namespace NotificationService.Models;

using System.Text.Json.Serialization;

public class NotificationEventPayload
{
    [JsonPropertyName("id")]
    public Guid NotificationId { get; set; }
    
    [JsonPropertyName("message")]
    public string? NotificationMessage { get; set; }
    
    [JsonPropertyName("type")]
    public string? NotificationType { get; set; }
    
    [JsonPropertyName("createdat")]
    public DateTime NotificationCreatedAt { get; set; }
}