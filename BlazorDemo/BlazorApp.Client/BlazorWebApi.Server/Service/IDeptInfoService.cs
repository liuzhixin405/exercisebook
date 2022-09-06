using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Service
{
    public interface IDeptInfoService
    {
        public List<DeptInfo> GetAll();
        public DeptInfo GetById(int deptId);
    }
}
