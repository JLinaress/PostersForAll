// This is a bridge between HTTP requests and the business logic or services that process those requests
namespace NotificationService.Controllers;

using Contracts;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IKafkaProducerService _kafkaProducerService;
    
    public NotificationController(INotificationService notificationService, IKafkaProducerService kafkaProducerService)
    {
        _notificationService = notificationService;
        _kafkaProducerService = kafkaProducerService;
    }

    [HttpPost]
    public async Task<IActionResult> SendKafkaNotification([FromBody] NotificationEventPayload payload)
    {
        var notification = _notificationService.CreateAndSendNotificationAsync(payload.NotificationMessage!, payload.NotificationType!, payload.NotificationId);

        if (notification == null)
        {
            return StatusCode(500, "Failed to create notification");
        }
        
        await _kafkaProducerService.ProduceNotificationEventAsync(
            payload.NotificationId.ToString(),
            JsonSerializer.Serialize(payload));
        
        return CreatedAtAction(nameof(GetNotificationById), new { id = payload.NotificationId }, payload);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotifications()
    {
        var notifications = await _notificationService.GetAllNotificationsAsync();
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
            return NotFound();

        return Ok(notification);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddNotification([FromBody] Notification notification)
    {
        var createdNotification = await _notificationService.AddNotificationAsync(notification);

        return CreatedAtAction(nameof(GetNotificationById), new { id = createdNotification.Id }, createdNotification);
    }
}