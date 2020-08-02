namespace AccuWeather.ResponseModels
{
    public class ResponseCurrentWeather
    {
        public string WeatherText { get; set; }
        public TemperatureTypes Temperature { get; set; }
    }

    public class TemperatureTypes
    {
        public TemperatureDetails Metric { get; set; }
        public TemperatureDetails Imperial { get; set; }
    }

    public class TemperatureDetails
    {
        public decimal Value { get; set; }
        public string Unit { get; set; }
    }

}
