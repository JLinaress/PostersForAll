using OrderService.Models;

namespace OrderService.Contracts;

public interface IOrderService
{
    Task<Order?> GetOrderByIdAsync(int id);
    
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    
    Task<Order> CreateOrderAsync(Order order);
    
    Task<bool> CancelOrderAsync(int id);
}