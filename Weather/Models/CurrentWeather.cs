using Weather.Interfaces;

namespace Weather.Models
{
    public class CurrentWeather : ICurrentWeather
    {
        public string WeatherDescription { get; set; }
        public decimal TemperatureValue { get; set; }
        public string TemperatureUnit { get; set; }
    }
}