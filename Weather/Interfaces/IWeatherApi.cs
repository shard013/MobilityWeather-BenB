using System.Collections.Generic;

namespace Weather.Interfaces
{
    public interface IWeatherApi
    {
        List<ICity> SearchCityByQuery(ICitySearch citySearch);
        ICurrentWeather GetCurrentWeather(ICity city);
    }
}
