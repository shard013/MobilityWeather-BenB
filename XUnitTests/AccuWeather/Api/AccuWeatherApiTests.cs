using Moq;
using AccuWeather.Api;
using System.Collections.Generic;
using Weather.Config;
using Weather.Interfaces;
using Weather.Models;
using Xunit;

namespace XUnitTests.AccuWeather.Api
{
    public class AccuWeatherApiTests
    {
        public WeatherConfig GetDefaultWeatherConfig()
        {
            var defaultConfig = new WeatherConfig
            {
                ActiveWeatherApi = "AccuWeather",
                WeatherProviders = new List<WeatherProvider>
                {
                    new WeatherProvider()
                    {
                        Name = "AccuWeather",
                        WeatherProviderDetails = new WeatherProviderDetails
                        {
                            ApiBase = "https://dataservice.accuweather.com",
                            ApiKey = "1234"
                        }
                    }
                }
            };
            return defaultConfig;
        }

        public Mock<INetworkClient> GetMockNetworkClient(string responseString)
        {
            var mockNetworkClient = new Mock<INetworkClient>();
            mockNetworkClient.Setup(c => c.GetString(It.IsAny<string>())).Returns(responseString);
            return mockNetworkClient;
        }

        [Theory]
        [InlineData("Perth", null, "https://dataservice.accuweather.com/locations/v1/search?apikey=1234&q=Perth")]
        [InlineData("Perth", "Uk", "https://dataservice.accuweather.com/locations/v1/search?apikey=1234&q=Perth%2cUk")]
        [InlineData("Sydney", null, "https://dataservice.accuweather.com/locations/v1/search?apikey=1234&q=Sydney")]
        public void RequestedSearchCityUriCorrect(string cityName, string extraQuery, string expectedUrl)
        {
            var weatherConfig = GetDefaultWeatherConfig();
            var api = new AccuWeatherApi(null, weatherConfig);
            var citySearch = new CitySearch
            {
                City = cityName,
                ExtraQuery = extraQuery
            };

            var results = api.GetSearchCityUri(citySearch);

            Assert.Equal(expectedUrl, results);
        }

        [Theory]
        [InlineData("[{\"Key\":\"26797\",\"Type\":\"City\",\"LocalizedName\":\"Perth\",\"Country\":{\"ID\":\"AU\",\"LocalizedName\":\"Australia\"}}]", "26797", "Perth")]
        [InlineData("[{\"Key\":\"22889\",\"Type\":\"City\",\"LocalizedName\":\"Sydney\",\"Country\":{\"ID\":\"AU\",\"LocalizedName\":\"Australia\"}}]", "22889", "Sydney")]
        public void DeserializeCitySearchJson(string networkJsonResponse, string expectedKey, string expectedName)
        {
            var mockNetworkClient = GetMockNetworkClient(networkJsonResponse);
            var weatherConfig = GetDefaultWeatherConfig();
            var api = new AccuWeatherApi(mockNetworkClient.Object, weatherConfig);

            var city = api.GetCitySearchResponse(string.Empty);

            Assert.Equal(expectedKey, city[0].Key);
            Assert.Equal(expectedName, city[0].LocalizedName);
        }

        [Theory]
        [InlineData("26797", "https://dataservice.accuweather.com/currentconditions/v1/26797?apikey=1234")]
        [InlineData("22889", "https://dataservice.accuweather.com/currentconditions/v1/22889?apikey=1234")]
        public void RequestedGetCurrentWeatherUriCorrect(string cityId, string expectedUrl)
        {
            var weatherConfig = GetDefaultWeatherConfig();
            var api = new AccuWeatherApi(null, weatherConfig);
            var city = new City
            {
                Key = cityId
            };

            var uri = api.GetCurrentWeatherUri(city);

            Assert.Equal(expectedUrl, uri);
        }

        public static IEnumerable<object[]> CurrentWeatherData =>
        new List<object[]>
        {
            new object[] {
                "[{\"WeatherText\":\"Clear\",\"Temperature\":{\"Metric\":{\"Value\":13.9,\"Unit\":\"C\",\"UnitType\":17},\"Imperial\":{\"Value\":57.0,\"Unit\":\"F\",\"UnitType\":18}}}]",
                "Clear",
                13.9M
            },
            new object[] {
                "[{\"WeatherText\":\"Cloudy\",\"Temperature\":{\"Metric\":{\"Value\":11.8,\"Unit\":\"C\",\"UnitType\":17},\"Imperial\":{\"Value\":53.0,\"Unit\":\"F\",\"UnitType\":18}}}]",
                "Cloudy",
                11.8M
            }
        };

        [Theory]
        [MemberData(nameof(CurrentWeatherData))]
        public void DeserializeCurrentWeatherJson(string networkJsonResponse, string expectedDescription, decimal expectedTemp)
        {
            var mockNetworkClient = GetMockNetworkClient(networkJsonResponse);
            var weatherConfig = GetDefaultWeatherConfig();
            var api = new AccuWeatherApi(mockNetworkClient.Object, weatherConfig);

            var currentWeather = api.GetCurrentWeatherResponse(string.Empty);

            Assert.Equal(expectedDescription, currentWeather.WeatherText);
            Assert.Equal(expectedTemp, currentWeather.Temperature.Metric.Value);
        }

    }
}
