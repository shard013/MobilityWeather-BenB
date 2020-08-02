using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Weather.Interfaces;

namespace Weather.Network
{
    public class NetworkClient : INetworkClient
    {
        const int HttpClientTimeout = 15;
        readonly List<string> InvalidApiStrings = new List<string> {
            "\"cod\":401",
            "\"Code\":\"Unauthorized\""
        };

        public string GetString(string requestUri)
        {
            var result = GetHttpResponse(requestUri).Content.ReadAsStringAsync().Result;

            var apiInvalidCount = InvalidApiStrings.Where(s => result.Contains(s)).Count();
            if (apiInvalidCount > 0)
            {
                throw new ApiKeyException("Invalid API key");
            }

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
