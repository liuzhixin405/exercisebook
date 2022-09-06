using BlazorApp.Shared;
using BlazorWebApi.Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Service
{
    public class UserInfoService : IUserInfoService
    {
        public UserInfo Add(UserInfo userInfo)
        {
            userInfo.UserID = DBStore.UserInfos.Count;

            DBStore.UserInfos.Add(userInfo);
            return userInfo;
        }

        public void Delete(int id)
        {
            var exist = DBStore.UserInfos.SingleOrDefault(x => x.UserID == id);
            if (exist == null)
            {
                throw new ArgumentNullException("id不存在");
            }
            DBStore.UserInfos.Remove(exist);

        }

        //public List<UserInfo> GetByDeptId(int deptId)
        //{
        //    return DBStore.UserInfos.Where(x => x.DeptId == deptId).ToList();
        //}

        public List<UserInfo> GetForDepartment(UserParameters userParameters)
        {
            var userInfos = DBStore.UserInfos;
            if (!string.IsNullOrWhiteSpace(userParameters?.SearchTerm))
            {
                userInfos = DBStore.UserInfos.Where(m => m.UserName.Contains(userParameters.SearchTerm)).ToList();
            }
            return userInfos;
        }

        public UserInfo GetOne(int deptId, int id)
        {
            return DBStore.UserInfos.SingleOrDefault(x => x.DeptId == deptId && x.UserID == id);
        }

        public void Update(UserInfo userInfo)
        {
            var exist = DBStore.UserInfos.SingleOrDefault(x => x.UserID == userInfo.UserID);
           
            DBStore.UserInfos.Remove(exist);
            DBStore.UserInfos.Add(userInfo);
        }
    }
}
