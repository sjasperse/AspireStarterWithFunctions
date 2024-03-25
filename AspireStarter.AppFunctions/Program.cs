using System.Text.Json;
using AspireStarter.AppFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .AddServiceDefaults()
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
