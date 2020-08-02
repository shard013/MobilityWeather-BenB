using System;
using System.Collections.Generic;
using System.Linq;
using Weather.Config;
using Weather.Interfaces;
using Weather.Models;

namespace MobilityWeather_BenB.Weather
{
    class WeatherApplication
    {
        readonly IWeatherApi _weatherApi;
        readonly SearchHistory _searchHistory;
        readonly WeatherUiOutput _output;
        readonly WeatherConfig _weatherConfig;

        public const int UiMaxHistorySize = 9; //Set the maximum history elements to display/handle, will not impact background storage

        public WeatherApplication(IWeatherApi weatherApi, SearchHistory searchHistory, WeatherUiOutput output, WeatherConfig weatherConfig)
        {
            _weatherApi = weatherApi;
            _searchHistory = searchHistory;
            _output = output;
            _weatherConfig = weatherConfig;
            SetHistoryPath();
        }

        public void Run()
        {
            MainMenuLoop();
        }

        void SetHistoryPath()
        {
            _searchHistory.SetHistoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        void MainMenuLoop()
        {
            while (true) //Keep looping until user exits
            {
                //Set display
                _output.PrintMainMenu(_weatherConfig.ActiveWeatherApi);
                _output.PrintSearchHistoryOptions(_searchHistory.History);
                _output.PrintSelectMainMenuOption();

                //Read user input
                var key = Console.ReadKey();

                //Handle user input
                if (key.Key == ConsoleKey.E)
                {
                    return;
                }
                HandleSearchMenuInput(key);
                HandleHistoryMenuInput(key);
            }
        }

        void HandleSearchMenuInput(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.S)
            {
                Search();
            }

            else if (key.Key == ConsoleKey.A)
            {
                Search(advancedSearch: true);
            }
        }

        void HandleHistoryMenuInput(ConsoleKeyInfo key)
        {
            var historyItemIndexSelected = -1;
            var historyItemsCount = Math.Min(UiMaxHistorySize, _searchHistory.History.Count); //handling anything over UiMaxHistorySize doesn't make sense with the current UI

            //Handle top row of number keys
            if (key.Key > ConsoleKey.D0 && key.Key <= ConsoleKey.D0 + historyItemsCount)
            {
                historyItemIndexSelected = key.Key - ConsoleKey.D0 - 1;
            }

            //Handle numeric pad
            else if (key.Key > ConsoleKey.NumPad0 && key.Key <= ConsoleKey.NumPad0 + historyItemsCount)
            {
                historyItemIndexSelected = key.Key - ConsoleKey.NumPad0 - 1;
            }

            var city = _searchHistory.History.ElementAtOrDefault(historyItemIndexSelected);
            if (city != null)
            {
                _output.PrintSearchHistorySpacing();
                ShowCurrentWeather(city);
            }
        }

        void Search(bool advancedSearch = false)
        {
            var citySearch = new CitySearch();

            _output.PrintSearchPrompt();
            citySearch.City = Console.ReadLine();

            if (advancedSearch)
            {
                _output.PrintAdvancedSearchPrompt();
                citySearch.ExtraQuery = Console.ReadLine();
            }

            _output.PrintSearchingInProgress();

            var cities = _weatherApi.SearchCityByQuery(citySearch);
            HandleSearchResults(cities);
        }

        void HandleSearchResults(List<ICity> cities)
        {
            if (cities != null && cities.Count > 0)
            {
                var city = cities.First();
                _output.PrintFoundCity(city);
                ShowCurrentWeather(city);
            }
            else
            {
                _output.PrintNoCityFound();
                Console.ReadKey();
            }
        }

        void ShowCurrentWeather(ICity city)
        {
            _searchHistory.MoveCityToTop(city);

            _output.PrintGettingCurrentWeather(city);

            var weather = _weatherApi.GetCurrentWeather(city);

            _output.PrintCurrentWeather(weather);

            Console.ReadKey();
        }

    }
}
