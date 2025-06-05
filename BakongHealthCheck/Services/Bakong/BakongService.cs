using System.Net.Http;
using BakongHealthCheck.APIConnection;
using BakongHealthCheck.Configures;
using BakongHealthCheck.Dto.Bakong;
using Newtonsoft.Json;
using Serilog;

namespace BakongHealthCheck.Services.Bakong
{
    public class BakongService : IBakongService
    {
        private readonly IMyHttpRequest myHttpRequest;
        private readonly IConfigureBakong configure;
    
        public BakongService(IMyHttpRequest myHttpRequest, IConfigureBakong configure)
        {
            this.myHttpRequest = myHttpRequest;
            this.configure = configure;
        }

        public Task<ResponseBakongHealthDTO> GetBakongHealth()
        {
            try
            {
                string a = configure.BakongBaseUrl.ToString();
                Log.Information("BakongHealthCheck > Direct to Bakong | Start");
                var response = myHttpRequest.HttpReqstApiB24Async<ResponseBakongHealthDTO>("", configure.BakongBaseUrl.ToString(), configure.BakongHealthCheck.ToString(), "");
                Log.Information("BakongHealthCheck > Direct to Bakong response : " + response);
                return response;
            }
            catch (Exception ex)
            {
                Log.Error("BakongHealthCheck > Direct to Bakong |" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

    }
}

