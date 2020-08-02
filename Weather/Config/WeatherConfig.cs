using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Weather.Config
{
    [JsonObject("WeatherConfig")]
    public class WeatherConfig
    {
        [JsonProperty("ActiveWeatherApi")]
        public string ActiveWeatherApi { get; set; }

        [JsonProperty("WeatherProviders")]
        public List<WeatherProvider> WeatherProviders { get; set; }

        [JsonProperty("MaxHistorySize")]
        public int MaxHistorySize { get; set; } = 5; //Defaults to 5 if not provided in config. appsettings.json value will override

        public WeatherProvider GetProviderConfig(string provider)
        {
            return WeatherProviders?.FirstOrDefault(w => w.Name == provider);
        }
    }

    public class WeatherProvider
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("WeatherProviderDetails")]
        public WeatherProviderDetails WeatherProviderDetails { get; set; }
    }

    public class WeatherProviderDetails
    {
        [JsonProperty("ApiBase")]
        public string ApiBase { get; set; }

        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; }
    }
}
