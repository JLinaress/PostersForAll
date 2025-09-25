// Business logic for managing orders
// This class will implement methods to place, retrieve, and cancel orders


namespace OrderService.Services;

using Microsoft.EntityFrameworkCore;
using Contracts;
using Data;
using Models;

public class OrderService : IOrderService
{
    private readonly OrdersContext _context;
    
    public OrderService(OrdersContext context)
    {
        _context = context;
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