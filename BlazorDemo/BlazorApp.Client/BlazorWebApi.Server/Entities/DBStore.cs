using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Entities
{
    public class DBStore
    {
        public static List<DeptInfo> DeptInfos = new DeptInfoEntities().DeptInfos;
        public static List<UserInfo> UserInfos = new UserInfoEntities().UserInfos;
    }
}
