using System.Collections.Generic;
using Weather.Config;
using Xunit;

namespace XUnitTests.Weather.Config
{
    public class WeatherConfigTests
    {
        public WeatherConfig GetDefaultWeatherConfig()
        {
            var defaultConfig = new WeatherConfig
            {
                ActiveWeatherApi = "Provider01",
                WeatherProviders = new List<WeatherProvider>
                {
                    new WeatherProvider()
                    {
                        Name = "Provider01",
                        WeatherProviderDetails = new WeatherProviderDetails()
                    },
                    new WeatherProvider()
                    {
                        Name = "Provider02",
                        WeatherProviderDetails = new WeatherProviderDetails()
                    }
                }
            };
            return defaultConfig;
        }

        [Fact]
        public void GetMissingProviderReturnsNull()
        {
            var weatherConfig = GetDefaultWeatherConfig();
            var providerConfig = weatherConfig.GetProviderConfig("Provider does not exist");
            Assert.Null(providerConfig);
        }

        [Fact]
        public void GetNullProviderReturnsNull()
        {
            var weatherConfig = GetDefaultWeatherConfig();
            var providerConfig = weatherConfig.GetProviderConfig(null);
            Assert.Null(providerConfig);
        }

        [Theory]
        [InlineData("Provider01", "Provider01")]
        [InlineData("Provider02", "Provider02")]
        public void GetValidProviderReturnsProviderConfig(string givenProvider, string expectedName)
        {
            var weatherConfig = GetDefaultWeatherConfig();
            var providerConfig = weatherConfig.GetProviderConfig(givenProvider);
            Assert.NotNull(providerConfig);
            Assert.Equal(expectedName, providerConfig.Name);
        }

        [Fact]
        public void GetProviderOnBlankConfigReturnsNull()
        {
            var weatherConfig = new WeatherConfig();
            var providerConfig = weatherConfig.GetProviderConfig(null);
            Assert.Null(providerConfig);
        }

    }
}
