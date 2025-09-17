// This is where we'll be registering our DbContext and other services for the InventoryService application.

using InventoryService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, Let's check our Inventory!");


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<InventoryContext>(options =>
            options.UseSqlite("Data Source=inventory.db"));
        
        //register other logic, background workers, repositories, etc.
    });

await builder.RunConsoleAsync();