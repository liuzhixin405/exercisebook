using IDCM.Contract.BaseInfo;
using IDCM.Contract.Client.Extension;
using IDCM.Contract.Foundation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Threading.Tasks;

namespace IDCM.Contract.Client.Controllers
{
    [ApiController]
    [Route("api/baseinfo/[action]")]
    public class BaseInfoController: ControllerBase
    {
        private readonly ILogger<BaseInfoController> _logger;
        private readonly IBaseDataGrains _baseDataGrains;
        private readonly IFoundationGrains _foundationGrains;

        public BaseInfoController(ILogger<BaseInfoController> logger)
        {
            _logger = logger;
            var serviceProvider = GlobalConfigure.ServiceLocatorInstance.CreateScope().ServiceProvider;
            this._foundationGrains = serviceProvider.GetRequiredService<IOrleansClient>().GetGrain<IFoundationGrains>(1);
            this._baseDataGrains = serviceProvider.GetRequiredService<IOrleansClient>().GetGrain<IBaseDataGrains>(1);
        }
        [HttpPost]
     
        public async Task<bool> GetError()
        {
            return await this._baseDataGrains.GetError();
        }

        [HttpGet]
        
        public string RtnString()
        {
            return "OK";
        }
        [HttpGet]

        public Task<bool> SaveOrder()
        {
            return _baseDataGrains.SaveOrder();
        }

        [HttpGet]

        public Task<bool> Foundation()
        {
            return _foundationGrains.GetError();
        }
    }
}
