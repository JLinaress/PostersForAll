// This is where we'll be registering our DbContext and other services for the OrderService application.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Data;
using Microsoft.Extensions.Hosting;


Console.WriteLine("Hello, Order Posters!");

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add services to container.
        services.AddDbContext<OrdersContext> (options => 
            options.UseSqlite("Data Source=orders.db"));
        
        //register other logic, background workers, repositories, etc.
    });
    
await builder.Build().RunAsync();