// Using Object-Relational Mapping (ORM) to bridge between SQL tables and objects-oriented entities.

using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models;

public class Notification
{
    public int Id { get; set; }
    
    [StringLength(50)] // Message max length 50 characters
    public string? Message { get; set; }
    
    [StringLength(10)] // Type max length 10 characters
    public string? Type { get; set; } // Success, Failure, Info
    
    public DateTime CreatedAt { get; set; }
}