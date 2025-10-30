// Register DbContext and Configure Dependency Injection

using Microsoft.EntityFrameworkCore;
using OrderService.Configuration;
using OrderService.Contracts;
using OrderService.Data;
using OrderService.Services;

Console.WriteLine("Hello, Let's check our Orders!");

var builder = WebApplication.CreateBuilder(args);
var kafkaConfig = builder.Configuration.GetSection("Kafka");
var bootstrapServers = kafkaConfig.GetValue<string>("BootstrapServers") ?? "localhost:9092";

// Add DbContext with SQLite
builder.Services.AddDbContext<OrdersContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

// Register other services, repositories, etc.
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

// Register Kafka Producer Service with environment variable configuration
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();