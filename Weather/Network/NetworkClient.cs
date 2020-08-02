using System;
using System.Net.Http;
using Weather.Interfaces;

namespace Weather.Network
{
    public class NetworkClient : INetworkClient
    {
        const int HttpClientTimeout = 15;

        public string GetString(string requestUri)
        {
            var result = GetHttpResponse(requestUri).Content.ReadAsStringAsync().Result;
            return result;
        }

        HttpResponseMessage GetHttpResponse(string requestUri)
        {
            HttpClient httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(HttpClientTimeout)
            };
            {
                var httpResponse = httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
                return httpResponse.Result;
            }
        }


    }
}
