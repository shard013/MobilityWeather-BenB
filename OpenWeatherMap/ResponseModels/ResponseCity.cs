namespace OpenWeatherMap.ResponseModels
{
    public class ResponseCity
    {
        public string Cod { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Sys Sys { get; set; }
    }

    public class Sys
    {
        public string Country { get; set; }
    }
}
