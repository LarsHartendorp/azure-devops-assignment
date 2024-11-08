﻿using System.Text;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StartJobFunction;

public class StartJobFunction
{
    private readonly ILogger<StartJobFunction> _logger;
    private readonly QueueClient _queueClient;
    public StartJobFunction(ILogger<StartJobFunction> logger, QueueClient queueClient)
    {
        _logger = logger;
        _queueClient = queueClient;
    }

    [Function("StartJobFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("Received a request to start a weather image generation job.");
        
        string jobId = Guid.NewGuid().ToString();
        var bytes = Encoding.UTF8.GetBytes(jobId);
        
        await _queueClient.CreateIfNotExistsAsync();
        await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
        _logger.LogInformation($"Job ID {jobId} added to the queue for processing.");
        
        var responseMessage = new { JobId = jobId};
        return new OkObjectResult(responseMessage);
    }
}