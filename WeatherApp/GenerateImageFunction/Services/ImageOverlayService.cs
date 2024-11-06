using System.Drawing;
using System.Drawing.Imaging;
using GenerateImageFunction.Models;

namespace GenerateImageFunction.Services
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
                var textBrush = new SolidBrush(Color.White);

                // Set up semi-transparent black/grey background brush
                var backgroundBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)); // 50% transparency, black color

                // Define overlay text
                var overlayText =
                    $"Naam: {weatherStation.StationName}\n" +
                    $"Plaats: {weatherStation.Region}\n" +
                    $"Temp: {weatherStation.Temperature}°C\n" +
                    $"Gevoels temp: {weatherStation.FeelTemperature}°C\n" +
                    $"Vochtigheid: {weatherStation.Humidity}%";

                // Measure the text size to fit background rectangle
                var textSize = graphics.MeasureString(overlayText, font);

                // Draw the semi-transparent background rectangle
                var backgroundRect = new RectangleF(5, 5, textSize.Width + 10, textSize.Height + 10);
                graphics.FillRectangle(backgroundBrush, backgroundRect);

                // Draw the overlay text on top of the background rectangle
                graphics.DrawString(overlayText, font, textBrush, new PointF(10, 10));
            }

            var resultStream = new MemoryStream();
            bitmap.Save(resultStream, ImageFormat.Png);
            resultStream.Position = 0; // Reset stream position for reading
            return resultStream;
        }
    }
}