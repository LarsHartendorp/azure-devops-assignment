using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using ExposeBlobFunction.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddScoped<SasTokenService>();
        services.AddSingleton(x => 
            new BlobServiceClient(Environment.GetEnvironmentVariable("BlobStorageConnection"))); 
    })
    .Build();

host.Run();