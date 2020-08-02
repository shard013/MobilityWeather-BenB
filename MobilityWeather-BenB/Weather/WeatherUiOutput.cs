using System;
using System.Collections.Generic;
using Weather.Interfaces;

namespace MobilityWeather_BenB.Weather
{
    public class WeatherUiOutput
    {
        public void PrintMainMenu(string providerName)
        {
            Console.Clear();
            Console.WriteLine($"Welcome to City Weather information - Provided by {providerName}");
            Console.WriteLine($"");
            Console.WriteLine($"[S] Search for a city");
            Console.WriteLine($"[A] Advanced search");
            Console.WriteLine($"[E] Exit");
            Console.WriteLine($"");
        }

        public void PrintSearchHistoryOptions(List<ICity> history)
        {
            if (history.Count > 0)
            {
                Console.WriteLine($"Previous search history");
                Console.WriteLine($"");

                var n = 0;
                foreach (var city in history)
                {
                    n++;
                    if (n > WeatherApplication.UiMaxHistorySize) //Displaying anything over UiMaxHistorySize doesn't make sense with the current UI
                    {
                        break;
                    }
                    Console.WriteLine($"[{n}] {city.Name}, {city.Country}");
                }

                Console.WriteLine($"");
            }
        }

        public void PrintSelectMainMenuOption()
        {
            Console.Write($"Select an option: ");
        }

        public void PrintSearchHistorySpacing()
        {
            Console.WriteLine($"");
            Console.WriteLine($"");
        }

        public void PrintSearchPrompt()
        {
            Console.Clear();
            Console.WriteLine($"Enter city name to search for and press Enter");
            Console.WriteLine($"");
            Console.Write($"City: ");
        }

        public void PrintAdvancedSearchPrompt()
        {
            Console.WriteLine($"");
            Console.WriteLine($"Enter additional search details");
            Console.WriteLine($"");
            Console.Write($"Additional details: ");
        }

        public void PrintSearchingInProgress()
        {
            Console.WriteLine($"");
            Console.Write($"Searching for city...");
        }
        public void PrintFoundCity(ICity city)
        {
            Console.WriteLine($" found {city.Name}, {city.Country}   [{city.Key}]");
            Console.WriteLine($"");
        }

        public void PrintNoCityFound()
        {
            Console.WriteLine($"");
            Console.WriteLine($"No cities found, press any key to return to the main menu");

        }

        public void PrintGettingCurrentWeather(ICity city)
        {
            Console.WriteLine($"Looking up current weather for {city.Name}, {city.Country}...");
            Console.WriteLine($"");
        }

        public void PrintCurrentWeather(ICurrentWeather weather)
        {
            Console.WriteLine($"Temperature: {weather.TemperatureValue}{weather.TemperatureUnit}");
            Console.WriteLine($"Conditions:  {weather.WeatherDescription}");
            Console.WriteLine($"");
            Console.WriteLine($"Press any key to return to the main menu");
        }

        public void PrintInvalidApiKey(string providerName)
        {
            Console.WriteLine($" *** There was an issue with the API key for {providerName}. ***");
            Console.WriteLine($"");
            Console.WriteLine($" *** Please check the appsettings.json config file. ***");
            Console.WriteLine($"");
            Console.WriteLine($"This program will now close");
        }

    }
}
