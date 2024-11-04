using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FunctionProcessWeatherImage.Models;

namespace FunctionGenerateWeatherImage.Services
{
    public class ImageOverlayService
    {
        public MemoryStream OverlayWeatherInfoOnImage(Stream imageStream, WeatherStation weatherStation)
        {
            using var originalImage = Image.FromStream(imageStream);
            using var bitmap = new Bitmap(originalImage);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                var font = new Font("Arial", 20);
                var brush = new SolidBrush(Color.Red);
                var overlayText = $"Temp: {weatherStation.Temperature}°C\n" +
                                  $"Feels Like: {weatherStation.FeelTemperature}°C\n" +
                                  $"Humidity: {weatherStation.Humidity}%";

                graphics.DrawString(overlayText, font, brush, new PointF(10, 10));
            }

            var resultStream = new MemoryStream();
            bitmap.Save(resultStream, ImageFormat.Png);
            resultStream.Position = 0; // Reset stream position for reading
            return resultStream;
        }
    }
}