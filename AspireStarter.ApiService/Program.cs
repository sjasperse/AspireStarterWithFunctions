using System.Diagnostics;
using System.Text;
using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
var messagesQueueClient = new QueueClient(builder.Configuration["QueueConnectionString"], "messages");
builder.Services.AddSingleton(messagesQueueClient);

await messagesQueueClient.CreateIfNotExistsAsync();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// middleware to send messages for each request
app.Use(async (context, next) => {
    var messageJson = System.Text.Json.JsonSerializer.Serialize(new {
        correlationId = Activity.Current!.Id,
        message = $"Request: {context.Request.Method.ToString().ToUpper()} {context.Request.Path}"
    });

    var toBase64 = (string str) => Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    await messagesQueueClient.SendMessageAsync(toBase64(messageJson));

    await next();
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
