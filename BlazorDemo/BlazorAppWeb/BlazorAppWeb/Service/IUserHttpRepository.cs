using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorAppWeb.Service
{
    public interface IUserHttpRepository
    {
        public Task<UserInfo> GetUserInfoById(int userId);
        public Task<List<UserInfo>> GetUserInfos();
        public Task<PagingResponse<UserInfo>> GetUserInfos(UserParameters userParameters);
        public Task<List<DeptInfo>> GetDeptInfos();
        public Task<UserInfo> AddUserinfo(UserInfo userinfo);
        public Task<UserInfo> UpdateUser(UserInfo userinfo);
        public Task<bool> DeleteUser(string userid);
        public Task<string> UploadFile(MultipartFormDataContent content);


    }
}
