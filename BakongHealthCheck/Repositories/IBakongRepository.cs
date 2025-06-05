using BakongHealthCheck.Dto;
using BakongHealthCheck.Dto.MBService;
using BakongHealthCheck.Entities;

namespace BakongHealthCheck.Repository
{
    public interface IBakongRepository
    {
        Task<ResponseV1DTO> getBakongHealth(string isUpdate);
        Task<ResponseV1DTO> createBakongHealthCheck();
        Task<ResponseV1DTO> createBakongHealthCheckTestcase();
    }
}
