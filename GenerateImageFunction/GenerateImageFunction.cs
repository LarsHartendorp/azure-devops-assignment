using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GenerateImageFunction;

public class GenerateImageFunction
{
    private readonly ILogger<GenerateImageFunction> _logger;

    public GenerateImageFunction(ILogger<GenerateImageFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(GenerateImageFunction))]
    public void Run([QueueTrigger("stationqueue", Connection = "")] QueueMessage message)
    {
        _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        
        // TODO: void --> Async Task maken. 
        
    }
}