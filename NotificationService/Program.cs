// Register DbContext and Configure Dependency Injection

using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationService.Configurations;
using NotificationService.Contracts;
using NotificationService.Data;
using NotificationService.Services;
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

// TODO : refactor 
builder.Services.Configure<KafkaConsumerSettings>(
    builder.Configuration.GetSection("KafkaConsumerSettings"));

builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<KafkaConsumerSettings>>().Value;
    var config = new ProducerConfig
    {
        BootstrapServers = settings.BootstrapServers ?? "localhost:9092",
        ClientId = "notification-service-producer",
        Acks = Acks.All,
        EnableIdempotence = true,
        MessageTimeoutMs = 5000
    };
    return new ProducerBuilder<string, string>(config).Build();
});

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
