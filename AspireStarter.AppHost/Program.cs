var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedisContainer("cache");

var apiservice = builder.AddProject<Projects.AspireStarter_ApiService>("apiservice");

builder.AddProject<Projects.AspireStarter_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiservice);

// not working yet on my machine.
// getting this in the function logs: "The gRPC channel URI 'http://:'; could not be parsed."
builder.AddProject<Projects.AspireStarter_AppFunctions>("functions")
    .WithReference(cache)
    .WithReference(apiservice);

builder.Build().Run();
