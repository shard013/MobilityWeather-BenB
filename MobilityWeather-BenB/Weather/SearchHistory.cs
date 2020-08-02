using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Weather.Config;
using Weather.Interfaces;
using Weather.Models;

namespace MobilityWeather_BenB.Weather
{
    public class SearchHistory
    {
        public int MaxHistorySize { get; }

        public List<ICity> History { get; private set; } = new List<ICity>();

        string FullPath { get; set; }

        readonly WeatherConfig _weatherConfig;

        public SearchHistory(WeatherConfig weatherConfig)
        {
            MaxHistorySize = weatherConfig.MaxHistorySize;
            _weatherConfig = weatherConfig;
        }

        public void MoveCityToTop(ICity city)
        {
            var cityInList = History.FirstOrDefault(c => c.Key == city.Key);

            if (cityInList != null) //bump city to top of list
            {
                History.Remove(cityInList);
                History.Insert(0, cityInList);
            }
            else //add new city to stop of list
            {
                History.Insert(0, city);
            }

            if (History.Count > MaxHistorySize) //trim history list to maximum if needed
            {
                History.RemoveRange(MaxHistorySize, History.Count - MaxHistorySize);
            }

            SaveHistory();
        }

        //Enables saving and loading to disk once set
        //Search history still functions as in memory only if not set
        public void SetHistoryPath(string path)
        {
            var filename = $"{_weatherConfig.ActiveWeatherApi}.history.json";
            path = Path.Combine(path, AppDomain.CurrentDomain.FriendlyName);
            Directory.CreateDirectory(path);
            FullPath = Path.Combine(path, filename);

            ReadHistory();
        }

        void ReadHistory()
        {
            if (FullPath == null)
            {
                return;
            }

            if (!File.Exists(FullPath))
            {
                SaveHistory();
            }

            var json = File.ReadAllText(FullPath);
            History = JsonConvert.DeserializeObject<List<City>>(json).ToList<ICity>();
        }

        void SaveHistory()
        {
            if (FullPath == null)
            {
                return;
            }

            var json = JsonConvert.SerializeObject(History);
            File.WriteAllText(FullPath, json);
        }

    }
}
