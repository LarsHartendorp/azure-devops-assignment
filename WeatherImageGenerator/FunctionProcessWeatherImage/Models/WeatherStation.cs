namespace FunctionProcessWeatherImage.Models;

public class WeatherStation
{    
    public string JobId { get; set; }
    public int StationId { get; set; }
    public string StationName { get; set; }
    public string Region { get; set; }
    public double Temperature { get; set; }
    public double FeelTemperature { get; set; }
    public double Humidity { get; set; }
}