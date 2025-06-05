using Serilog;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BakongHealthCheck.APIConnection
{
    public abstract class BaseHttpClientWithFactory
    {
        private readonly IHttpClientFactory _factory;
        public Uri BaseAddress { get; set; }
        public string BasePath { get; set; }
        public BaseHttpClientWithFactory(IHttpClientFactory factory)
        => _factory = factory;
        private HttpClient GetHttpClient()
        {
            return _factory.CreateClient();
        }
        public virtual async Task<T> SendRequest<T>(HttpRequestMessage request)
        where T : class
        {
            var client = GetHttpClient();
            var response = await client.SendAsync(request);
            T result = null;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                Log.Debug("Get or Post API Success ," + response);
                result = await response.Content.ReadAsAsync<T>(GetFormatters());
            }
            return result;
        }
        protected virtual IEnumerable<MediaTypeFormatter> GetFormatters()
        {
            // Make JSON the default
            return new List<MediaTypeFormatter> { new JsonMediaTypeFormatter() };
        }

        public virtual async Task<string> SendRequestStringAsync(HttpRequestMessage request)
        {
            string result = null;
            var client = GetHttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
                Log.Debug("Get or Post API Success ," + response.RequestMessage);
            }
            else
            {
                result = "EWB" + "-" + await response.Content.ReadAsStringAsync();
                Log.Debug("Something went wrong ," + response + "Status code " + response.StatusCode);
            }
            return result;
        }
        public virtual async Task<string> PostRequestAsync(HttpContent content, HttpRequestMessage message)
        {
            string result = null;
            var client = GetHttpClient();
            client.DefaultRequestHeaders.Authorization = message.Headers.Authorization;
            var response = await client.PostAsync(message.RequestUri, content);

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
                Log.Debug("Get or Post API Success ," + response.RequestMessage);
            }
            else
            {
                result = "EWB" + "-" + await response.Content.ReadAsStringAsync();
                Log.Warning("Something went wrong ," + await response.Content.ReadAsStringAsync() + "Status code " + response.StatusCode);
            }
            return result;
        }

    }
}
