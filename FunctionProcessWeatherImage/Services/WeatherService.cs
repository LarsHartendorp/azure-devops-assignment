using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FunctionProcessWeatherImage.Models;

namespace FunctionProcessWeatherImage.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<WeatherStation>> GetWeatherDataAsync()
        {
            var response = await _httpClient.GetStringAsync("https://data.buienradar.nl/2.0/feed/json");
            
            using JsonDocument doc = JsonDocument.Parse(response);
            var stationMeasurements = doc.RootElement
                .GetProperty("actual")
                .GetProperty("stationmeasurements");

            var weatherStations = new List<WeatherStation>();

            foreach (var station in stationMeasurements.EnumerateArray())
            {
                var weatherStation = new WeatherStation()
                {
                    StationId = station.TryGetProperty("stationid", out var stationId) ? stationId.GetInt32() : 0,
                    StationName = station.TryGetProperty("stationname", out var stationName) ? stationName.GetString() : "Unknown",
                    Region = station.TryGetProperty("regio", out var region) ? region.GetString() : "Unknown",
                    Temperature = station.TryGetProperty("temperature", out var temperature) ? temperature.GetDouble() : 0.0,
                    FeelTemperature = station.TryGetProperty("feeltemperature", out var feelTemperature) ? feelTemperature.GetDouble() : 0.0,
                    Humidity = station.TryGetProperty("humidity", out var humidity) ? humidity.GetDouble() : 0.0
                };

                weatherStations.Add(weatherStation);
            }

            return weatherStations;
        }
    }
}