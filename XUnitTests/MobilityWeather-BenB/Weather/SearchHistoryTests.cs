using MobilityWeather_BenB.Weather;
using Weather.Config;
using Weather.Models;
using Xunit;

namespace XUnitTests.MobilityWeather_BenB.Weather
{
    public class SearchHistoryTests
    {
        public const string NewCityKey = "80";
        public const int MaxSearchHistorySize = 5;

        public City GetNewCity()
        {
            return new City
            {
                Key = NewCityKey,
                Name = "NewCityName",
                Country = "NewCityCountry"
            };
        }

        readonly WeatherConfig _weatherConfig = new WeatherConfig() { MaxHistorySize = MaxSearchHistorySize };

        public SearchHistory GetSearchHistoryOfSize(int size)
        {
            var searchHistory = new SearchHistory(_weatherConfig);
            for (int i = 1; i <= size; i++)
            {
                searchHistory.MoveCityToTop(new City { Key = i.ToString() });
            }
            return searchHistory;
        }

        public SearchHistory GetBlankSearchHistory()
        {
            return new SearchHistory(_weatherConfig);
        }


        [Fact]
        public void EmptySearchHistoryHasEmptyHistoryList()
        {
            var searchHistory = GetBlankSearchHistory();
            Assert.NotNull(searchHistory.History);
            Assert.Empty(searchHistory.History);
        }


        [Fact]
        public void AddingNewCityToEmptySearchHistoryIsAdded()
        {
            var searchHistory = GetBlankSearchHistory();
            var newCity = GetNewCity();

            searchHistory.MoveCityToTop(newCity);

            Assert.Single(searchHistory.History);
            Assert.Equal(NewCityKey, searchHistory.History[0].Key);
        }

        [Fact]
        public void AddingNewCityToEmptySearchHistoryMultipleTimesDoesNotDuplicateInList()
        {
            var searchHistory = GetBlankSearchHistory();
            var newCity = GetNewCity();

            searchHistory.MoveCityToTop(newCity);
            searchHistory.MoveCityToTop(newCity);

            Assert.Single(searchHistory.History);
            Assert.Equal(NewCityKey, searchHistory.History[0].Key);
        }

        [Fact]
        public void SearchHistoryCanReachExpectedMaximumSize()
        {
            var searchHistory = GetSearchHistoryOfSize(MaxSearchHistorySize);
            Assert.Equal(MaxSearchHistorySize, searchHistory.History.Count);
        }

        [Fact]
        public void SearchHistoryWillNotExceedMaximumSize()
        {
            var searchHistory = GetSearchHistoryOfSize(MaxSearchHistorySize + 1);
            Assert.Equal(MaxSearchHistorySize, searchHistory.History.Count);
        }

        [Fact]
        public void AddingNewCityToFullHistoryListAddsToTop()
        {
            var searchHistory = GetSearchHistoryOfSize(MaxSearchHistorySize);
            var newCity = GetNewCity();

            searchHistory.MoveCityToTop(newCity);

            Assert.Equal(NewCityKey, searchHistory.History[0].Key);
        }

        [Fact]
        public void NewCityBumpsOldCityDownByOne()
        {
            var searchHistory = GetSearchHistoryOfSize(MaxSearchHistorySize);
            var newCity = GetNewCity();

            searchHistory.MoveCityToTop(newCity);

            Assert.Equal(MaxSearchHistorySize.ToString(), searchHistory.History[1].Key);
        }

    }
}
