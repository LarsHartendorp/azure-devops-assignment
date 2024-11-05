using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FunctionProcessWeatherImage.Services;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using FunctionGenerateWeatherImage.Services;
using FunctionProcessWeatherImage.Models;

namespace FunctionProcessWeatherImage
{
    public class GenerateImageFunction
    {
        private readonly ILogger<GenerateImageFunction> _logger;
        private readonly ImageService _imageService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ImageOverlayService _imageOverlayService;

        public GenerateImageFunction(
            ILogger<GenerateImageFunction> logger,
            ImageService imageService,
            ImageOverlayService imageOverlayService,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _imageService = imageService;
            _imageOverlayService = imageOverlayService;            
            _blobServiceClient = blobServiceClient;
        }

        [Function("GenerateImageFunction")]
        public async Task RunAsync([QueueTrigger("stationqueue", Connection = "AzureWebJobsStorage")] string queueMessage)
        {
            _logger.LogInformation($"Queue message received: {queueMessage}");

            WeatherStation weatherStation;

            try
            {
                weatherStation = JsonSerializer.Deserialize<WeatherStation>(queueMessage);
                if (weatherStation == null)
                {
                    _logger.LogError("Deserialization failed, weatherStation is null.");
                    return;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Deserialization error: {ex.Message}");
                return;
            }

            // Get an image from Lorem Picsum
            var imageData = await _imageService.GetRandomImageAsync();

            // Overlay the weather information on the image
            using var imageStream = new MemoryStream(imageData);
            using var overlaidImageStream = _imageOverlayService.OverlayWeatherInfoOnImage(imageStream, weatherStation);

            // Define the container and blob name
            var containerClient = _blobServiceClient.GetBlobContainerClient("weather-images");
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient($"{weatherStation.JobId}/{weatherStation.StationId}.jpg");

            // Upload the overlaid image to Blob Storage
            overlaidImageStream.Position = 0; // Reset the stream position for reading
            await blobClient.UploadAsync(overlaidImageStream, overwrite: true);
    
            _logger.LogInformation($"Image uploaded to Blob Storage with URI: {blobClient.Uri}");
        }
    }
}