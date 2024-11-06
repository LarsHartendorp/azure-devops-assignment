using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ExposeBlobFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExposeBlobFunction
{
    public class ExposeBlobFunction
    {
        private readonly ILogger<ExposeBlobFunction> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly SasTokenService _sasTokenService;

        public ExposeBlobFunction(ILogger<ExposeBlobFunction> logger, BlobServiceClient blobServiceClient, SasTokenService sasTokenService)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
            _sasTokenService = sasTokenService;
        }

        [Function("ExposeBlobFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "images")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to fetch all blobs with SAS tokens.");

            // Get the container client
            var containerClient = _blobServiceClient.GetBlobContainerClient("weather-images");

            // List to store blob URIs with SAS tokens
            var blobUrisWithSas = new List<string>();

            // List blobs in the container
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                // Get the blob client
                var blobClient = containerClient.GetBlobClient(blobItem.Name);

                // Generate a SAS token for each blob (valid for 1 hour)
                var sasUri = _sasTokenService.GenerateSasToken(blobClient, TimeSpan.FromHours(1));
                if (sasUri != null)
                {
                    blobUrisWithSas.Add(sasUri.ToString());
                }
            }

            // Check if any blobs were found
            if (blobUrisWithSas.Count == 0)
            {
                return new NotFoundResult(); // No blobs found
            }

            // Return the list of blob URIs with SAS tokens
            return new OkObjectResult(blobUrisWithSas);
        }
    }
}
