// Register DbContext and Configure Dependency Injection

using Microsoft.EntityFrameworkCore;
using NotificationService.Contracts;
using NotificationService.Data;
using NotificationServices = NotificationService.Services.NotificationService;

Console.WriteLine("Hello, You've got a Notification!");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add DbContext with SQLite
builder.Services.AddDbContext<NotificationContext>(options =>
    options.UseSqlite("Data Source=notifications.db"));

// Register other services, repositories, etc.
builder.Services.AddScoped<INotificationService, NotificationServices>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
