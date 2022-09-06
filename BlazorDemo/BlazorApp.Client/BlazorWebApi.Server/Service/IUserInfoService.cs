using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Service
{
    public interface IUserInfoService
    {
        public List<UserInfo> GetForDepartment(UserParameters userParameters);
        public UserInfo GetOne(int deptId, int id);
        public UserInfo Add(UserInfo userInfo);
        public void Update(UserInfo userInfo);
        public void Delete(int id);
        //public List<UserInfo> GetByDeptId(int deptId);
    }
}
