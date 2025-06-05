using BakongHealthCheck.Entities;
using BakongHealthCheck.Repositories;
using BakongHealthCheck.Data;
using BakongHealthCheck.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure;
using Serilog;
using BakongHealthCheck.Dto.Bakong;
using System;
using BakongHealthCheck.Services.Bakong;
using AutoMapper;
using BakongHealthCheck.Dto.MBService;
using BakongHealthCheck.Dto;

namespace BakongHealthCheck.Repositories
{
    public class BakongRepository : IBakongRepository
    {
        private readonly AppDBContext appDbContext;
        private readonly IBakongService bakongService;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IMapper mapper;


        public BakongRepository(AppDBContext appDbContext, IBakongService bakongService, 
            IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.bakongService = bakongService;
            this.scopeFactory = scopeFactory;
            this.mapper = mapper;
        }

        public async Task<ResponseV1DTO> getBakongHealth(string isUpdate)
        {
            var result = new ResponseV1DTO();
            try
            {
                // for open service again ( httprequest already close connect by singleton )
                var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                var response = await service.mbService.OrderByDescending(b => b.recID).
                                    FirstOrDefaultAsync(b => b.remark == "LASTED" && b.status != "UP");
                if (response == null)
                {
                    // No insert here 
                    Log.Information("BakongHealthCheck > getBakongHealthCheck from DB |Data response null ");
                    return result = new ResponseV1DTO()
                    {
                        responseCode = "2000",
                        responseMessage = "Bakong service is : UP!!"
                    };
                }

                if ( isUpdate == "YES" )
                {
                    // Update last data for bakong back to normal
                    response.userID = response.userID;
                    response.serviceID = response.serviceID;
                    response.startDate = response.startDate;
                    response.endDate = response.endDate;
                    response.status = response.status;
                    response.blackListVersion = response.blackListVersion;
                    response.remark = "";
                    service.Update(response);
                    await service.SaveChangesAsync();
                    Log.Information("BakongHealthCheck > getBakongHealthCheck from DB |Data response " + response);
                    result = new ResponseV1DTO()
                    {
                        responseCode = "2000",
                        responseMessage = "Bakong service is : UP!!"
                    };
                }
                else
                {
                    Log.Information("BakongHealthCheck > getBakongHealthCheck from DB |Data response " + response);
                    result = new ResponseV1DTO()
                    {
                        responseCode = "2001",
                        responseMessage = "Bakong service is : " + response.status,
                    };
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Information("BakongHealthCheck > getBakongHealthCheck from DB |Catch error : " + ex.Message);
                return new ResponseV1DTO()
                {
                    responseCode = "4000",
                    responseMessage = ex.Message
                };
            }
        }

        public async Task<ResponseV1DTO> createBakongHealthCheck()
        {
            var result = new ResponseV1DTO();
            try
            {
                ResponseBakongHealthDTO bakongHealth = await bakongService.GetBakongHealth();
                if (bakongHealth == null)
                {
                    Log.Information("BakongHealthCheck > createBakongHealthCheck > got status from bakong api null.");
                    return result = new ResponseV1DTO()
                    {
                        responseCode = "4000",
                        responseMessage = "Bakong health check not found!!"
                    };
                }

                if (bakongHealth.code == "000")
                {
                    Log.Information("BakongHealthCheck > createBakongHealthCheck > getBakongHealthV2 : " + bakongHealth.result.status);
                    return await getBakongHealth("YES");
                }
                else
                {
                    ResponseV1DTO response = await getBakongHealth("NO");
                    var scope = scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                    // Response 2000 ( Query returned no results from the database, so need to insert )
                    if (response.responseCode == "2000")
                    {
                        RequestMBServiceDTO request = new RequestMBServiceDTO()
                        {
                            userID = "DENY",
                            serviceID = "BKCH00001",
                            startDate = DateTime.Now,
                            endDate = DateTime.Now,
                            status = bakongHealth.result.status,
                            remark = "LASTED"
                        };

                        var requestData = mapper.Map<MBService>(request);

                        await service.mbService.AddAsync(requestData);
                        await service.SaveChangesAsync();
                    }

                    Log.Information("BakongHealthCheck > createBakongHealthCheck > Bakong down and already insert DB.");

                    result = new ResponseV1DTO()
                    {
                        responseCode = "2001",
                        responseMessage = "Bakong is : " + bakongHealth.result.status + "!!"
                    };

                    return result;      
                }              
            }
            catch (Exception ex)
            {
                Log.Information("BakongHealthCheck > createBakongHealthCheck > Catch Error " + ex.Message);
                return new ResponseV1DTO()
                {
                    responseCode = "4000",
                    responseMessage = ex.Message
                };
            }
        }
        
        public async Task<ResponseV1DTO> createBakongHealthCheckTestcase()
        {
            var result = new ResponseV1DTO();
            try
            {
                ResponseBakongHealthDTO bakongHealth = await bakongService.GetBakongHealth();
                if (bakongHealth == null)
                {
                    Log.Information("BakongHealthCheck > createBakongHealthCheck > got status from bakong api null.");
                    return result = new ResponseV1DTO()
                    {
                        responseCode = "4000",
                        responseMessage = "Bakong health check not found!!"
                    };
                }

                if (bakongHealth.code != "000")
                {
                    Log.Information("BakongHealthCheck > createBakongHealthCheck > getBakongHealthV2 : " + bakongHealth.result.status);
                    return await getBakongHealth("YES");
                }
                else
                {
                    ResponseV1DTO response = await getBakongHealth("NO");
                    var scope = scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                    // Response 2000 ( Query returned no results from the database, so need to insert )
                    if (response.responseCode == "2000")
                    {
                        RequestMBServiceDTO request = new RequestMBServiceDTO()
                        {
                            userID = "DENY",
                            serviceID = "BKCH00001",
                            startDate = DateTime.Now,
                            endDate = DateTime.Now,
                            status = bakongHealth.result.status,
                            remark = "LASTED"
                        };

                        var requestData = mapper.Map<MBService>(request);

                        await service.mbService.AddAsync(requestData);
                        await service.SaveChangesAsync();
                    }

                    Log.Information("BakongHealthCheck > createBakongHealthCheck > Bakong down and already insert DB.");

                    result = new ResponseV1DTO()
                    {
                        responseCode = "2001",
                        responseMessage = "Bakong is : " + bakongHealth.result.status + "!!"
                    };

                    return result;
                }
            }
            catch (Exception ex)
            {
                Log.Information("BakongHealthCheck > createBakongHealthCheck > Catch Error " + ex.Message);
                return new ResponseV1DTO()
                {
                    responseCode = "4000",
                    responseMessage = ex.Message
                };
            }
        }


    }
}
