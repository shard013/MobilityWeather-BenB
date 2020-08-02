using Weather.Interfaces;

namespace Weather.Models
{
    public class CitySearch : ICitySearch
    {
        public string City { get; set; }

        public string ExtraQuery { get; set; }
    }
}
