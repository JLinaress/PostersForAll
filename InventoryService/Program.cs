// Register DbContext and Configure Dependency Injection

using InventoryService.Contracts;
using InventoryService.Data;
using Microsoft.EntityFrameworkCore;
using InventoryServices = InventoryService.Services.InventoryService;

Console.WriteLine("Hello, Lets check our Inventory!");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add DbContext with SQLite
builder.Services.AddDbContext<InventoryContext>(options => 
    options.UseSqlite("Data Source=inventory.db"));
    
// Register other services, repositories, etc.
builder.Services.AddScoped<IInventoryService, InventoryServices>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();