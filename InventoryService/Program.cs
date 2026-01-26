// Register DbContext and Configure Dependency Injection
using InventoryService.Contracts;
using InventoryService.Data;
using InventoryService.Messaging.Consumers;
using InventoryService.Messaging.HandlerMessage;
using InventoryService.Services;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using InventoryServices = InventoryService.Services.InventoryService;

Console.WriteLine("Hello, Lets check our Inventory!");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    // Use launchSettings.json port for local development (5224)
    // Dev mode message
    Console.WriteLine("Local Dev Mode detected. Using launchSettings.json port.");
}
else
{
    // Kubernetes deployment : Force port 8080 binding : PROD
    builder.WebHost.UseUrls("http://+:8080"); // Listen on port 8080
}
// This adds the DiagnosticContext service
builder.Host.UseSerilog();
var kafkaConfig = builder.Configuration.GetSection("Kafka");
var bootstrapServers = kafkaConfig.GetValue<string>("BootstrapServers") ?? "localhost:9092";

// Add services to the container.
builder.Services.AddOpenApi();

// Add DbContext with SQLite
builder.Services.AddDbContext<InventoryContext>(options => 
    options.UseSqlite("Data Source=inventory.db"));
    
// Register other services, repositories, etc.
builder.Services.AddScoped<IInventoryService, InventoryServices>();

// Register Kafka Producer Service with environment variable configuration
builder.Services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddSingleton<IMessageHandlerService, MessageHandlerService>();

// Adding Swagger to inspect your API endpoints / data
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/Health", () => "Inventory Service is healthy");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventoryContext>();
    context.Database.EnsureCreated();  // Creates tables based on OnModelCreating
}

// This will give you a swagger browser to test GET/POST 
// TODO
// this is a Swagger middleware and need to come back and ensure it's only active in development mode
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryService V1");
    c.RoutePrefix = string.Empty;
});

//serilog request logging middleware
app.UseSerilogRequestLogging();
// Collect HTTP metrics
app.UseHttpMetrics();
// Expose the /metrics endpoint for Prometheus to scrape
app.UseMetricServer();

app.MapControllers();

app.Run();