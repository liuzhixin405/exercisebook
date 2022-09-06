using BlazorApp.Shared;
using BlazorWebApi.Server.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Controllers
{
    [ApiController]
    [Route("dept")]
    public class DeptController : ControllerBase
    {
        private IDeptInfoService _deptInfoService;
        public DeptController(IDeptInfoService deptInfoService)
        {
            _deptInfoService = deptInfoService;
        }

        [HttpGet]
        [Route("GetDeptInfos")]
        public IList<DeptInfo> GetDeptInfos()
        {
            return _deptInfoService.GetAll(); ;
        }
    }
}
