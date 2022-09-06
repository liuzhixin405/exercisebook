using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Entities
{
    public class DeptInfoEntities
    {
        public List<DeptInfo> DeptInfos;
        public DeptInfoEntities()
        {
			DeptInfos = new List<DeptInfo>();
			DeptInfos.Add(new DeptInfo()
			{
				DeptId = 1,
				Name = "研发部"
			});
			DeptInfos.Add(new DeptInfo()
			{
				DeptId = 2,
				Name = "法务部"
			});
			DeptInfos.Add(new DeptInfo()
			{
				DeptId = 3,
				Name = "业务部"
			});
		}
    }
}
