// Logic layer interface for order management operations

namespace OrderService.Contracts;

using Models;

public interface IOrderService
{
    Task<Order?> GetOrderByIdAsync(int id);
    
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    
    Task<Order> CreateOrderAsync(Order order);
    
    Task<bool> CancelOrderAsync(int id);
}