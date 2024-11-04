using System.Collections.Generic;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FunctionProcessWeatherImage.Services;
using FunctionProcessWeatherImage.Models;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace FunctionProcessWeatherImage
{
    public class ProcessWeatherImageFunction
    {
        private readonly ILogger<ProcessWeatherImageFunction> _logger;
        private readonly WeatherService _weatherService;

        public ProcessWeatherImageFunction(ILogger<ProcessWeatherImageFunction> logger, WeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [Function(nameof(ProcessWeatherImageFunction))]
        public async Task Run([QueueTrigger("jobstartqueue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            // Fetch weather data
            var stationMeasurements = await _weatherService.GetWeatherDataAsync();
            _logger.LogInformation($"Processing station measurements: {string.Join(", ", stationMeasurements)}");
            
            // foreach weatherstation --> new message op de queue
            // Nieuwe azure functie die die message oppikt
            // voor elk weerstation een image ophalen en dat combineren
            // in de blobstorage opslaan
            
            // als dat gelukt is, dan een nieuwe functie die de blobs (per id) kan ophalen
            
        }
    }
}