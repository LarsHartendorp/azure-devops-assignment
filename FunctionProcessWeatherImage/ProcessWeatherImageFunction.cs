using System.Text;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FunctionProcessWeatherImage.Services;
using System.Text.Json;
using Azure.Storage.Queues;

namespace FunctionProcessWeatherImage
{
    public class ProcessWeatherImageFunction
    {
        private readonly ILogger<ProcessWeatherImageFunction> _logger;
        private readonly WeatherService _weatherService;
        private readonly QueueClient _stationQueueClient;

        public ProcessWeatherImageFunction(
            ILogger<ProcessWeatherImageFunction> logger, 
            WeatherService weatherService, 
            QueueClient stationQueueClient)
        {
            _logger = logger;
            _weatherService = weatherService;
            _stationQueueClient = stationQueueClient;
        }

        [Function(nameof(ProcessWeatherImageFunction))]
        public async Task Run([QueueTrigger("jobstartqueue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation($"Queue trigger function processed: {message.MessageText}");

            // Fetch weather data
            var weatherStations = await _weatherService.GetWeatherDataAsync();
            _logger.LogInformation($"Aantal weerstations opgehaald: {weatherStations.Count}");
            
            // Iterate over each WeatherStation and send a message to the station queue
            foreach (var station in weatherStations)
            {
                var weatherStationMessage = JsonSerializer.Serialize(station);
                Console.WriteLine(weatherStationMessage);
                
                var bytes = Encoding.UTF8.GetBytes(weatherStationMessage);

                // Send the encoded message to the station queue
                await _stationQueueClient.CreateIfNotExistsAsync();
                await _stationQueueClient.SendMessageAsync(Convert.ToBase64String(bytes));
                _logger.LogInformation($"Sent message to station queue for station: {station.StationName}");
            }
        }
    }
}
