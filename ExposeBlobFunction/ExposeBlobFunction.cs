using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExposeBlobFunction;

public class ExposeBlobFunction
{
    private readonly ILogger<ExposeBlobFunction> _logger;
    private readonly BlobServiceClient _blobServiceClient;

    public ExposeBlobFunction(ILogger<ExposeBlobFunction> logger, BlobServiceClient blobServiceClient)
    {
        _logger = logger;
        _blobServiceClient = blobServiceClient;
    }

    [Function("ExposeBlobFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "images")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request to fetch all blobs.");

        // Get the container client
        var containerClient = _blobServiceClient.GetBlobContainerClient("weather-images");

        // Create a list to store the blob URIs
        var blobUris = new List<string>();

        // List blobs in the container
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            // Add the URI of each blob to the list
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            blobUris.Add(blobClient.Uri.ToString());
        }

        // Check if any blobs were found
        if (blobUris.Count == 0)
        {
            return new NotFoundResult(); // No blobs found
        }

        // Return the list of blob URIs
        return new OkObjectResult(blobUris);
    }
}
