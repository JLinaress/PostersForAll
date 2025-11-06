// Creating a message handler service for inventory messages instead of depending on Func delegates.
namespace InventoryService.Messaging.HandlerMessage;

using Contracts;

public class MessageHandlerService : IMessageHandlerService
{
    public Task HandleMessageAsync(string message)
    {
        // Simulate message handling logic
        Console.WriteLine($"Handling message: {message}");
        return Task.CompletedTask;
    }
}