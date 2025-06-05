using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BakongHealthCheck.Dto;
using BakongHealthCheck.Dto.MBService;
using BakongHealthCheck.Repository;
using Serilog;

namespace BakongHealthCheck.Services
{
    public class BCService : IBCService
    {
        private readonly IBakongRepository _repository;

        public BCService(IBakongRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseV1DTO> BakongHealthCheck()
        {
            try
            {
                Log.Information("BakongHealthCheck > BackgroundProcess start");
                ResponseV1DTO response = await _repository.createBakongHealthCheck();
            }
            catch (Exception ex)
            {
                Log.Information("BakongHealthCheck > BackgroundProcess catch error : " + ex.Message);
            }
            finally {
                Log.Information("BakongHealthCheck > BackgroundProcess end");
            }
            return null;
        }


        public async Task<ResponseV1DTO> BakongHealthCheckTestcase()
        {
            try
            {
                Log.Information("BakongHealthCheck > BackgroundProcess start");
                ResponseV1DTO response = await _repository.createBakongHealthCheckTestcase();
            }
            catch (Exception ex)
            {
                Log.Information("BakongHealthCheck > BackgroundProcess catch error : " + ex.Message);
            }
            finally
            {
                Log.Information("BakongHealthCheck > BackgroundProcess end");
            }
            return null;
        }

    }
}
