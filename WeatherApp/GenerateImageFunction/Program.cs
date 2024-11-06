using Azure.Storage.Blobs;
using Azure.Storage.Queues;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var stationQueueName = Environment.GetEnvironmentVariable("StationQueueName");
        var blobConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");

        // Register QueueClient for stationqueue
        services.AddSingleton(new QueueClient(connectionString, stationQueueName));

        // Register BlobServiceClient
        services.AddSingleton(new BlobServiceClient(blobConnectionString));

        // Register WeatherService
        services.AddHttpClient<WeatherService>();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpClient();
        services.AddSingleton<ImageService>();
        services.AddSingleton<ImageOverlayService>();
    })
    .Build();

host.Run();