using BlazorApp.Shared;
using BlazorWebApi.Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Service
{
    public class DeptInfoService : IDeptInfoService
    {
        public List<DeptInfo> GetAll()
        {
           return DBStore.DeptInfos;
        }

        public DeptInfo GetById(int deptId)
        {
            return DBStore.DeptInfos.SingleOrDefault(x => x.DeptId == deptId);
        }
    }
}
