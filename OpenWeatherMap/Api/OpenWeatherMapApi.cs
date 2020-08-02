using Newtonsoft.Json;
using OpenWeatherMap.ResponseModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Weather.Config;
using Weather.Interfaces;
using Weather.Models;

namespace OpenWeatherMap.Api
{
    public class OpenWeatherMapApi : IWeatherApi
    {
        public const string ConfigIdentifier = "OpenWeatherMap";

        const string UnitRequest = "metric";
        const string UnitDisplayTemperature = "C";
        const string CityFoundCode = "200";

        const string PhQuery = "{query}";
        const string PhCityKey = "{cityKey}";
        const string PhApiKey = "{apiKey}";
        const string PhUnitRequest = "{unitRequest}";

        readonly string SearchCityByQueryEndpoint = $"/data/2.5/weather?q={PhQuery}&appid={PhApiKey}&units={PhUnitRequest}";
        readonly string GetCurrentWeatherEndpoint = $"/data/2.5/weather?id={PhCityKey}&appid={PhApiKey}&units={PhUnitRequest}";

        readonly INetworkClient _networkClient;
        readonly WeatherProviderDetails Config;

        public OpenWeatherMapApi(INetworkClient networkClient, WeatherConfig weatherConfig)
        {
            _networkClient = networkClient;
            Config = weatherConfig.GetProviderConfig(ConfigIdentifier).WeatherProviderDetails;
        }

        public List<ICity> SearchCityByQuery(ICitySearch citySearch)
        {
            var requestUri = GetSearchCityUri(citySearch);
            var response = GetCitySearchResponse(requestUri);

            var cities = new List<ICity>();
            if (response.Cod == CityFoundCode)
            {
                var city = new City
                {
                    Key = response.Id,
                    Name = response.Name,
                    Country = response.Sys.Country
                };
                cities.Add(city);
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
                .Replace(PhQuery, HttpUtility.UrlEncode(queryString))
                .Replace(PhUnitRequest, HttpUtility.UrlEncode(UnitRequest));

            return requestUri;
        }

        public ResponseCity GetCitySearchResponse(string requestUri)
        {
            var json = _networkClient.GetString(requestUri);
            var response = JsonConvert.DeserializeObject<ResponseCity>(json);
            return response;
        }


        public ICurrentWeather GetCurrentWeather(ICity city)
        {
            var requestUri = GetCurrentWeatherUri(city);
            var response = GetCurrentWeatherResponse(requestUri);

            var currentWeather = new CurrentWeather
            {
                TemperatureValue = response.Main.Temp,
                TemperatureUnit = UnitDisplayTemperature
            };

            if (response.Weather.Count > 0)
            {
                currentWeather.WeatherDescription = response.Weather.First().Description;
            }

            return currentWeather;
        }

        public string GetCurrentWeatherUri(ICity city)
        {
            var requestUri = $"{Config.ApiBase}{GetCurrentWeatherEndpoint}"
                                .Replace(PhApiKey, Config.ApiKey)
                                .Replace(PhCityKey, HttpUtility.UrlEncode(city.Key))
                                .Replace(PhUnitRequest, HttpUtility.UrlEncode(UnitRequest));
            return requestUri;
        }

        public ResponseCurrentWeather GetCurrentWeatherResponse(string requestUri)
        {
            var json = _networkClient.GetString(requestUri);
            var response = JsonConvert.DeserializeObject<ResponseCurrentWeather>(json);
            return response;
        }

    }
}
