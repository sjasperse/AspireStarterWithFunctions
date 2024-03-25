using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using OpenTelemetry.Trace;

namespace AspireStarter.FunctionApp;

public class OpenTelemetryMiddleware : IFunctionsWorkerMiddleware
{
    private static ActivitySource activitySource = new ActivitySource(
        Assembly.GetExecutingAssembly().GetName().Name!,
        "1.0.0");

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var parentId = GetCorrelationId(context);

        using var activity = activitySource.StartActivity(
            context.FunctionDefinition.Name,
            ActivityKind.Consumer,
            parentId: parentId);

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
        finally
        {
            activity?.Stop();
        }
    }

    private string? GetCorrelationId(FunctionContext context)
    {
        if (context.BindingContext.BindingData.ContainsKey("QueueTrigger"))
        {
            var message = context.BindingContext.BindingData["QueueTrigger"] as string;
            var messageDict = JsonSerializer.Deserialize<Dictionary<string, object>>(message!)!;

            if (messageDict.TryGetValue("correlationId", out var value))
            {
                if (value != null)
                {
                    return value.ToString();
                }
            }
        }

        return null;
    }
}
