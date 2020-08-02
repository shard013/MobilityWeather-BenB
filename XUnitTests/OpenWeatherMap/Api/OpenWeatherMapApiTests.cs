using Moq;
using OpenWeatherMap.Api;
using System.Collections.Generic;
using Weather.Config;
using Weather.Interfaces;
using Weather.Models;
using Xunit;

namespace XUnitTests.OpenWeatherMap.Api
{
    public class OpenWeatherMapApiTests
    {
        public WeatherConfig GetDefaultWeatherConfig()
        {
            var defaultConfig = new WeatherConfig
            {
                ActiveWeatherApi = "OpenWeatherMap",
                WeatherProviders = new List<WeatherProvider>
                {
                    new WeatherProvider()
                    {
                        Name = "OpenWeatherMap",
                        WeatherProviderDetails = new WeatherProviderDetails
                        {
                            ApiBase = "https://api.openweathermap.org",
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
        [InlineData("Perth", null, "https://api.openweathermap.org/data/2.5/weather?q=Perth&appid=1234&units=metric")]
        [InlineData("Perth", "Uk", "https://api.openweathermap.org/data/2.5/weather?q=Perth%2cUk&appid=1234&units=metric")]
        [InlineData("Sydney", null, "https://api.openweathermap.org/data/2.5/weather?q=Sydney&appid=1234&units=metric")]
        [InlineData("القاهرة", null, "https://api.openweathermap.org/data/2.5/weather?q=%d8%a7%d9%84%d9%82%d8%a7%d9%87%d8%b1%d8%a9&appid=1234&units=metric")]
        public void RequestedSearchCityUriCorrect(string cityName, string extraQuery, string expectedUrl)
        {
            var weatherConfig = GetDefaultWeatherConfig();
            var api = new OpenWeatherMapApi(null, weatherConfig);
            var citySearch = new CitySearch
            {
                City = cityName,
                ExtraQuery = extraQuery
            };

            var uri = api.GetSearchCityUri(citySearch);

            Assert.Equal(expectedUrl, uri);
        }

        [Theory]
        [InlineData("{\"sys\":{\"country\":\"AU\",},\"id\":2063523,\"name\":\"Perth\",\"cod\":200}", "2063523", "Perth")]
        [InlineData("{\"sys\":{\"country\":\"AU\",},\"id\":2147714,\"name\":\"Sydney\",\"cod\":200}", "2147714", "Sydney")]
        public void DeserializeCitySearchJson(string networkJsonResponse, string expectedKey, string expectedName)
        {
            var mockNetworkClient = GetMockNetworkClient(networkJsonResponse);
            var weatherConfig = GetDefaultWeatherConfig();
            var api = new OpenWeatherMapApi(mockNetworkClient.Object, weatherConfig);

            var city = api.GetCitySearchResponse(string.Empty);

            Assert.Equal(expectedKey, city.Id);
            Assert.Equal(expectedName, city.Name);
        }


        [Theory]
        [InlineData("2063523", "https://api.openweathermap.org/data/2.5/weather?id=2063523&appid=1234&units=metric")]
        [InlineData("2147714", "https://api.openweathermap.org/data/2.5/weather?id=2147714&appid=1234&units=metric")]
        public void RequestedGetCurrentWeatherUriCorrect(string cityId, string expectedUrl)
        {
            var weatherConfig = GetDefaultWeatherConfig();
            var api = new OpenWeatherMapApi(null, weatherConfig);
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
            new object[] { "{\"weather\":[{\"description\":\"clear\"}],\"main\":{\"temp\":23.45}}", "clear", 23.45M },
            new object[] { "{\"weather\":[{\"description\":\"broken clouds\"}],\"main\":{\"temp\":15.00}}", "broken clouds", 15.00M }
        };

        [Theory]
        [MemberData(nameof(CurrentWeatherData))]
        public void DeserializeCurrentWeatherJson(string networkJsonResponse, string expectedDescription, decimal expectedTemp)
        {
            var mockNetworkClient = GetMockNetworkClient(networkJsonResponse);
            var weatherConfig = GetDefaultWeatherConfig();
            var api = new OpenWeatherMapApi(mockNetworkClient.Object, weatherConfig);

            var currentWeather = api.GetCurrentWeatherResponse(string.Empty);

            Assert.Equal(expectedDescription, currentWeather.Weather[0].Description);
            Assert.Equal(expectedTemp, currentWeather.Main.Temp);
        }

    }
}
