namespace Weather.Interfaces
{
    public interface ICurrentWeather
    {
        string WeatherDescription { get; }
        decimal TemperatureValue { get; }
        string TemperatureUnit { get; }
    }
}
