// Register DbContext and Configure Dependency Injection

using Microsoft.EntityFrameworkCore;
using OrderService.Contracts;
using OrderService.Data;

Console.WriteLine("Hello, Let's check our Orders!");

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQLite
builder.Services.AddDbContext<OrdersContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

// Register other services, repositories, etc.
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();