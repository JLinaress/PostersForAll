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
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ILogger<NotificationController> _logger;
    private readonly INotificationService _notificationService;
    
    public NotificationController(IKafkaProducerService kafkaProducerService, ILogger<NotificationController> logger, INotificationService notificationService)
    {
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<IActionResult> SendKafkaNotification([FromBody] NotificationEventPayload payload)
    {
        _logger.LogInformation($"Sending Kafka notification to {payload.NotificationId}");
        
        var notification = _notificationService.CreateAndSendNotificationAsync(payload.NotificationMessage!, payload.NotificationType!, payload.NotificationId);

        if (notification == null)
        {
            _logger.LogError($"Failed to send notification to {payload.NotificationId}");
            
            return StatusCode(500, "Failed to create notification");
        }
        
        _logger.LogInformation($"Sent notification to {payload.NotificationId}");
        
        await _kafkaProducerService.ProduceNotificationEventAsync(
            payload.NotificationId.ToString(),
            JsonSerializer.Serialize(payload));
        
        return CreatedAtAction(nameof(GetNotificationById), new { id = payload.NotificationId }, payload);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotifications()
    {
        _logger.LogInformation("Getting all notifications");
        
        var notifications = await _notificationService.GetAllNotificationsAsync();
        
        _logger.LogInformation($"Returning {notifications.Count} notifications");
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        _logger.LogInformation($"Getting notification {id}");
        
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
        {
            _logger.LogWarning($"Notification {id} not found");
            return NotFound();
        }

        _logger.LogInformation($"Returning notification {id}");
        return Ok(notification);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddNotification([FromBody] Notification notification)
    {
        _logger.LogInformation("Adding new notification");
        var createdNotification = await _notificationService.AddNotificationAsync(notification);

        _logger.LogInformation($"Notification {createdNotification.Id} added successfully");
        return CreatedAtAction(nameof(GetNotificationById), new { id = createdNotification.Id }, createdNotification);
    }
}