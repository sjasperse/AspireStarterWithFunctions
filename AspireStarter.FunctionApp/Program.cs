using System.Text.Json;
using AspireStarter.FunctionApp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .AddServiceDefaultsForFunctionApp()
    .ConfigureFunctionsWorkerDefaults((IFunctionsWorkerApplicationBuilder c) => {
        c.UseMiddleware<OpenTelemetryMiddleware>();
    })
    .ConfigureServices((context, services) => {
        services.AddSingleton(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
    })
    .Build();
    
host.Run();
