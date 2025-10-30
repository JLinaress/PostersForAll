// Business logic for managing inventory
// This class will implement methods to create, retrieve, and cancel orders

using Newtonsoft.Json;

namespace OrderService.Services;

using Microsoft.EntityFrameworkCore;
using Contracts;
using Data;
using Models;

public class OrderService : IOrderService
{
    private readonly OrdersContext _context;
    private readonly IKafkaProducerService _kafkaProducerService;
    
    public OrderService(OrdersContext context, IKafkaProducerService kafkaProducerService)
    {
        _context = context;
        _kafkaProducerService = kafkaProducerService;
    }
    
    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders.FindAsync(id);
    }
    
    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }
    
    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.Status = "Placed";
        order.CreatedAt = DateTime.UtcNow;
        
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        
        // Publish kafka event 
        var orderPlacedEvent = JsonConvert.SerializeObject(new
        {
            OrderId = order.Id,
            order.ItemSku,
            order.Quantity,
            order.Status,
            order.CreatedAt
        });

        // publish to event after order is created
        await _kafkaProducerService.ProduceOrderEventMessageAsync(order.Id.ToString(), orderPlacedEvent);
        
        return order;
    }
    
    public async Task<bool> CancelOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null || order.Status == "Cancelled")
        {
            return false;
        }
        
        order.Status = "Cancelled";
        await _context.SaveChangesAsync();
        
        return true;
    }
}