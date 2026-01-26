// Register DbContext and Configure Dependency Injection
using Microsoft.EntityFrameworkCore;
using NotificationService.Configurations;
using NotificationService.Contracts;
using NotificationService.Data;
using NotificationService.Messaging.Consumer;
using NotificationService.Services;
using Prometheus;
using Serilog;
using NotificationServices = NotificationService.Services.NotificationService;

Console.WriteLine("Hello, You've got a Notification!");

// configure serilog
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
    // Use launchSettings.json port for local development (5199)
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

// Add services to the container.
builder.Services.AddOpenApi();

// Add DbContext with SQLite
builder.Services.AddDbContext<NotificationContext>(options =>
    options.UseSqlite("Data Source=notifications.db"));

// Register other services, repositories, etc.
builder.Services.AddScoped<INotificationService, NotificationServices>();

// Register the underlying kafka IProducer client
builder.Services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

// TODO : refactor 
builder.Services.Configure<KafkaConsumerSettings>(
    builder.Configuration.GetSection("KafkaConsumerSettings"));

// Adding Swagger to inspect your API endpoints / data
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/Health", () => "Notification Service is healthy");

//serilog request logging middleware
app.UseSerilogRequestLogging();
// Collect HTTP metrics
app.UseHttpMetrics();
// Expose the /metrics endpoint for Prometheus to scrape
app.UseMetricServer();

app.MapControllers();

app.Run();
