// Register DbContext and Configure Dependency Injection
using Microsoft.EntityFrameworkCore;
using OrderService.Configuration;
using OrderService.Contracts;
using OrderService.Data;
using OrderService.Services;
using Prometheus;
using Serilog;
using Serilog.Events;

Console.WriteLine("Hello, Let's check our Orders!");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    // Use launchSettings.json port for local development (5198)
    // Dev mode message
    Console.WriteLine("Local Dev Mode detected. Using launchSettings.json port.");
}
else
{
    // Kubernetes deployment : Force port 8080 binding : PROD
    builder.WebHost.UseUrls("http://+:8080"); // Listen on port 8080
}
builder.Host.UseSerilog();
var kafkaConfig = builder.Configuration.GetSection("Kafka");
var bootstrapServers = kafkaConfig.GetValue<string>("BootstrapServers") ?? "localhost:9092";

// Add DbContext with SQLite
builder.Services.AddDbContext<OrdersContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

// Register other services, repositories, etc.
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();

// Register Kafka Producer Service with environment variable configuration
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/Health", () => Results.Ok(new { status = "Order Service is healthy" }));

//serilog request logging middleware
app.UseSerilogRequestLogging();
// Collect HTTP metrics
app.UseHttpMetrics();

// Expose the /metrics endpoint for Prometheus to scrape
app.UseMetricServer();

app.MapControllers();

app.Run();