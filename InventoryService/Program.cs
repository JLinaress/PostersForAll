// Register DbContext and Configure Dependency Injection
using Confluent.Kafka;
using InventoryService.Configurations;
using InventoryService.Contracts;
using InventoryService.Data;
using InventoryService.Messaging.Clients;
using InventoryService.Messaging.Consumers;
using InventoryService.Messaging.HandlerMessage;
using InventoryService.Services;
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

// Register the underlying kafka IConsumer and IProducer client
builder.Services.AddSingleton<IConsumer<string, string>>(sp =>
{
    var config = new ConsumerConfig
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "localhost:9092",
        GroupId = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP_ID") ?? "inventory-service-consumer",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
    return new ConsumerBuilder<string, string>(config).Build();
});

builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "localhost:9092",
        ClientId = "inventory-service-producer"
    };
    return new ProducerBuilder<string, string>(config).Build();
});

// Register Kafka Producer Service with environment variable configuration
builder.Services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddSingleton<IMessageHandlerService, MessageHandlerService>();
builder.Services.Configure<KafkaConsumerSettings>(builder.Configuration.GetSection("KafkaConsumerSettings"));

// Kafka client wrapper
builder.Services.AddSingleton<IKafkaConsumerClient, KafkaConsumerClient>();
builder.Services.AddSingleton<IKafkaProducerClient, KafkaProducerClient>();

// Adding Swagger to inspect your API endpoints / data
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// This will give you a swagger browser to test GET/POST 
// TODO
// this is a Swagger middleware and need to come back and ensure it's only active in development mode
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryService V1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();