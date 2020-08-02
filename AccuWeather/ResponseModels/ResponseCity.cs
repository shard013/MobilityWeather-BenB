namespace AccuWeather.ResponseModels
{
    public class ResponseCity
    {
        public string Key { get; set; }
        public string LocalizedName { get; set; }
        public LocationType Type { get; set; }
        public Country Country { get; set; }
    }

    public enum LocationType
    {
        City,
        PostalCode,
        POI,
        LatLong
    }

    public class Country
    {
        public string LocalizedName { get; set; }
    }
}
