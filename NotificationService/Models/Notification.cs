// Using Object-Relational Mapping (ORM) to bridge between SQL tables and objects-oriented entities.

namespace NotificationService.Models;

public class Notification
{
    public int Id { get; set; }
    
    public string Message { get; set; }
    
    public string Type { get; set; } //Success, Error, Info
    
    public DateTime CreatedAt { get; set; }
}