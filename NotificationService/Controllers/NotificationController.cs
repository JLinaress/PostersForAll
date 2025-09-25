// This is a bridge between HTTP requests and the business logic or services that process those requests

namespace NotificationService.Controllers;

using Microsoft.AspNetCore.Mvc;
using Contracts;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<IActionResult> SendNotification([FromQuery] string message, [FromQuery] string type)
    {
        await _notificationService.SendNotificationAsync(message, type);
        return Ok(new { Status = "Notification Sent" });
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
    public async Task<IActionResult> AddNotification([FromBody] Models.Notification notification)
    {
        var createdNotification = await _notificationService.AddNotificationAsync(notification);

        return CreatedAtAction(nameof(GetNotificationById), new { id = createdNotification.Id }, createdNotification);
    }
}