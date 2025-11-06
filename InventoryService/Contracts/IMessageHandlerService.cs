// Message handler service interface responsible for asynchronously processing incoming messages 
namespace InventoryService.Contracts;

public interface IMessageHandlerService
{
    Task HandleMessageAsync(string message);
}