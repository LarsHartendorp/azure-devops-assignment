using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WeatherImageGenerator
{
    public static class StartJobFunction
    {
        [FunctionName("StartJobFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "start-job")] HttpRequest req,
            [Queue("weather-job-queue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> queueCollector,
            ILogger log)
        {
            log.LogInformation("Received a request to start a weather image generation job.");

            // Generate a unique job ID
            string jobId = Guid.NewGuid().ToString();

            // Add a message to the queue for background processing
            await queueCollector.AddAsync(jobId);
            log.LogInformation($"Job ID {jobId} added to the queue for processing.");

            // Return a response with the job ID
            var responseMessage = new { JobId = jobId};
            return new OkObjectResult(responseMessage);
        }
    }
}
