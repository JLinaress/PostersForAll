// This is a bridge between HTTP requests and the business logic or services that process those requests
// API Endpoints for managing orders, including creating, retrieving, and canceling orders.
namespace OrderService.Controllers;

using Microsoft.AspNetCore.Mvc;
using Contracts;
using Models;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    
    public OrderController(ILogger<OrderController> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        _logger.LogInformation("Received request to get order with id {OrderId}", id);       
        
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            _logger.LogWarning("Order with id {OrderId} not found", id);
            return NotFound();
        }

        _logger.LogInformation("Order with id {OrderId} retrieved successfully", id);
        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        _logger.LogInformation("Received request to get all orders");
        
        var orders = await _orderService.GetAllOrdersAsync();
        
        _logger.LogInformation("Returning {OrderCount} orders", orders.Count());
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] Order order)
    {
        _logger.LogInformation("Received request to place new order");

        var createdOrder = await _orderService.CreateOrderAsync(order);
        
        _logger.LogInformation("Order with id {OrderId} created successfully", createdOrder.Id);
        
        return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        _logger.LogInformation("Received request to cancel order with id {OrderId}", id);
        
        var result = await _orderService.CancelOrderAsync(id);
        if (!result)
        {
            _logger.LogWarning("Order with id {OrderId} not found for cancellation", id);
            return NotFound();
        }
        
        _logger.LogInformation("Order with id {OrderId} cancelled successfully", id);
        return NoContent();
    }
}