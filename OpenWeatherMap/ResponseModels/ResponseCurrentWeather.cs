using System.Collections.Generic;

namespace OpenWeatherMap.ResponseModels
{
    public class ResponseCurrentWeather
    {
        public List<Weather> Weather { get; set; }
        public Main Main { get; set; }
    }

    public class Weather
    {
        public string Description { get; set; }
    }

    public class Main
    {
        public decimal Temp { get; set; }
    }

}
