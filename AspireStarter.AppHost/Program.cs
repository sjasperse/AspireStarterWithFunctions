using AspireStarter.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var azurite = builder.AddContainer("azurite", "mcr.microsoft.com/azure-storage/azurite")
    .WithEndpoint(containerPort: 10000, name: "blob", hostPort: 11000)
    .WithEndpoint(containerPort: 10001, name: "queue", hostPort: 11001)
    .WithEndpoint(containerPort: 10001, name: "table", hostPort: 11002);
var queue = azurite.GetEndpoint("queue");
var queueConnStrCallback = () =>  $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;QueueEndpoint={queue.Value.Replace("tcp", "http")}/devstoreaccount1;";

var apiService = builder.AddProject<Projects.AspireStarter_ApiService>("apiservice")
    .WithReference(queue)
    .WithEnvironment("QueueConnectionString", queueConnStrCallback);


builder.AddProject<Projects.AspireStarter_Web>("webfrontend")
    .WithReference(apiService);

builder.AddAzureFunction<Projects.AspireStarter_FunctionApp>("functionApp", 7121, 7122)
    .WithReference(queue)
    .WithEnvironment("QueueConnectionString", queueConnStrCallback);

builder.Build().Run();
