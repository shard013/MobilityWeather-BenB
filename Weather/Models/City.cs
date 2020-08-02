using Weather.Interfaces;

namespace Weather.Models
{
    public class City : ICity
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

    }
}
