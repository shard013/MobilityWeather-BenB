using AccuWeather.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MobilityWeather_BenB.Weather;
using OpenWeatherMap.Api;
using System;
using Weather.Config;
using Weather.Interfaces;
using Weather.Network;

namespace MobilityWeather_BenB
{
    class Program
    {
        static IServiceProvider _serviceProvider;
        static IConfiguration _configuration;

        const string AppSettingsFile = "appsettings.json";
        const string AppSettingsWeatherSection = "WeatherConfig";

        static void Main(string[] args)
        {
            _configuration = RegisterConfigurations(args);
            _serviceProvider = ConfigureServices();

            IServiceScope scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<WeatherApplication>().Run();
            DisposeServices();
        }

        static IConfiguration RegisterConfigurations(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
              .AddJsonFile(AppSettingsFile, optional: false, reloadOnChange: false)
              .AddEnvironmentVariables()
              .AddCommandLine(args)
              .AddUserSecrets<Program>();

            var configuration = configurationBuilder.Build();

            return configuration;
        }

        static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<WeatherApplication>();
            services.AddSingleton<WeatherUiOutput>();
            services.AddSingleton<SearchHistory>();
            services.AddSingleton<INetworkClient, NetworkClient>();
            AddWeatherProvider(services);

            var serviceProvider = services.BuildServiceProvider(true);

            return serviceProvider;
        }

        static void AddWeatherProvider(IServiceCollection services)
        {
            var weatherConfig = new WeatherConfig();
            _configuration.Bind(AppSettingsWeatherSection, weatherConfig);
            services.AddSingleton(weatherConfig);

            switch (weatherConfig.ActiveWeatherApi)
            {
                case OpenWeatherMapApi.ConfigIdentifier:
                    services.AddSingleton<IWeatherApi, OpenWeatherMapApi>();
                    break;
                case AccuWeatherApi.ConfigIdentifier:
                    services.AddSingleton<IWeatherApi, AccuWeatherApi>();
                    break;
                default:
                    var errorMessage = "Misconfigured appsettings.json file. Value WeatherConfig ActiveWeatherApi must be set to known provider.";
                    Console.WriteLine(errorMessage);
                    Console.ReadLine();
                    throw new ApplicationException(errorMessage);
            }
        }

        static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }

            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

    }
}
