using FunctionProcessWeatherImage.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.AddHttpClient<WeatherService>(); 
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();