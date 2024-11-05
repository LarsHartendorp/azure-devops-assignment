using Azure.Storage.Queues;
using FunctionProcessWeatherImage.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // register queue client
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var queueName = Environment.GetEnvironmentVariable("StationQueueName");

        services.AddSingleton(new QueueClient(connectionString, queueName));
        
        services.AddHttpClient<WeatherService>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();