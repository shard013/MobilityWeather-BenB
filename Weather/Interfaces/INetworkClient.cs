namespace Weather.Interfaces
{
    public interface INetworkClient
    {
        string GetString(string requestUri);
    }
}
