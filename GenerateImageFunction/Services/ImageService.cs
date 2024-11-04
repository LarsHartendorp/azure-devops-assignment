using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionProcessWeatherImage.Services
{
    public class ImageService
    {
        private readonly HttpClient _httpClient;

        public ImageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> GetRandomImageAsync()
        {
            // Lorem Picsum API for a random image with a specific size, you can change size as needed
            var url = "https://picsum.photos/600/400";
            
            // Retrieve the image
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            // Read image as a byte array
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}