// This is where we'll be registering our DbContext and other services for the NotificationService application.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Data;

Console.WriteLine("Hello, You've got a Notification!");


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add services to container.
        services.AddDbContext<NotificationContext> (options => 
            options.UseSqlite("Data Source=notification.db"));
        
        //register other logic, background workers, repositories, etc.
    });

await builder.Build().RunAsync();