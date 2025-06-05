
using Newtonsoft.Json;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

namespace BakongHealthCheck.APIConnection
{
    public class MyHttpRequest : BaseHttpClientWithFactory, IMyHttpRequest
    {

        public MyHttpRequest(IHttpClientFactory factory) : base(factory)
        {

        }

        public async Task<TResponse> HttpReqstApiB24Async<TResponse>(string jsonRequest, string url, string urlPath, string keyAuth)
        {
            try
            {
                Log.Debug("Service request api : " + jsonRequest);
                var message = new HttpRequestBuilder(url)
                                  .SetPath(urlPath)
                                  .HttpMethod(HttpMethod.Get)
                                  .GetHttpMessage();
                message.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                string result = await SendRequestStringAsync(message);
                Log.Information("Service response: " + result);
                return JsonConvert.DeserializeObject<TResponse>(result);
            }
            catch (Exception ex)
            {
                Log.Debug("Service request api catch error : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
