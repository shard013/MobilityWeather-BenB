using AccuWeather.ResponseModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Weather.Config;
using Weather.Interfaces;
using Weather.Models;

namespace AccuWeather.Api
{
    public class AccuWeatherApi : IWeatherApi
    {
        public const string ConfigIdentifier = "AccuWeather";

        //Placeholders to use in replaces when generating URIs
        const string PhQuery = "{query}";
        const string PhCityKey = "{cityKey}";
        const string PhApiKey = "{apiKey}";

        readonly string SearchCityByQueryEndpoint = $"/locations/v1/search?apikey={PhApiKey}&q={PhQuery}";
        readonly string GetCurrentWeatherEndpoint = $"/currentconditions/v1/{PhCityKey}?apikey={PhApiKey}";
        
        readonly INetworkClient _networkClient;
        readonly WeatherProviderDetails Config;

        public AccuWeatherApi(INetworkClient networkClient, WeatherConfig weatherConfig)
        {
            _networkClient = networkClient;
            Config = weatherConfig.GetProviderConfig(ConfigIdentifier).WeatherProviderDetails;
        }

        public List<ICity> SearchCityByQuery(ICitySearch citySearch)
        {
            var requestUri = GetSearchCityUri(citySearch);
            var response = GetCitySearchResponse(requestUri);

            var cities = new List<ICity>();
            foreach (var rCity in response)
            {
                if (rCity.Type == LocationType.City)
                {
                    var city = new City
                    {
                        Key = rCity.Key,
                        Name = rCity.LocalizedName,
                        Country = rCity.Country.LocalizedName
                    };
                    cities.Add(city);
                }
            }

            return cities;
        }

        public string GetSearchCityUri(ICitySearch citySearch)
        {
            var queryString = citySearch.City;
            if (!string.IsNullOrWhiteSpace(citySearch.ExtraQuery))
            {
                queryString += $",{citySearch.ExtraQuery}";
            }

            var requestUri = $"{Config.ApiBase}{SearchCityByQueryEndpoint}"
                .Replace(PhApiKey, Config.ApiKey)
                .Replace(PhQuery, HttpUtility.UrlEncode(queryString));

            return requestUri;
        }

        public List<ResponseCity> GetCitySearchResponse(string requestUri)
        {
            var json = _networkClient.GetString(requestUri);
            var response = JsonConvert.DeserializeObject<List<ResponseCity>>(json);
            return response;
        }


        public ICurrentWeather GetCurrentWeather(ICity city)
        {
            var requestUri = GetCurrentWeatherUri(city);
            var response = GetCurrentWeatherResponse(requestUri);

            var currentWeather = new CurrentWeather
            {
                TemperatureValue = response.Temperature.Metric.Value,
                TemperatureUnit = response.Temperature.Metric.Unit,
                WeatherDescription = response.WeatherText
            };

            return currentWeather;
        }

        public string GetCurrentWeatherUri(ICity city)
        {
            var requestUri = $"{Config.ApiBase}{GetCurrentWeatherEndpoint}"
                    .Replace(PhApiKey, Config.ApiKey)
                    .Replace(PhCityKey, HttpUtility.UrlEncode(city.Key));

            return requestUri;
        }

        public ResponseCurrentWeather GetCurrentWeatherResponse(string requestUri)
        {
            var json = _networkClient.GetString(requestUri);
            var response = JsonConvert.DeserializeObject<List<ResponseCurrentWeather>>(json).First();
            return response;
        }

    }
}
