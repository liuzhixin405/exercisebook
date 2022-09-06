using BlazorApp.Shared;
using BlazorWebApi.Server.Paging;
using BlazorWebApi.Server.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private IUserInfoService _userInfoService;
        public UserController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        [HttpGet]
        [Route("GetByDeptId/{deptId}")]
        public IList<UserInfo> GetAllForDepartment(int deptId)
        {
            return _userInfoService.GetForDepartment(new UserParameters()).Where(x => x.DeptId == deptId).ToList();
        }
        [HttpGet]
        [Route("GetUser/{userId}")]
        public UserInfo GetUser(int userId)
        {
            return _userInfoService.GetForDepartment(new UserParameters()).Where(m => m.UserID == userId).SingleOrDefault();
        }
        [HttpPost]
        [Route("AddUser")]
        public UserInfo AddUser(UserInfo userInfo)
        {
            return _userInfoService.Add(userInfo);
        }

        [HttpGet]
        [Route("GetAll")]
        public List<UserInfo> GetAll()
        {
            return _userInfoService.GetForDepartment(new UserParameters());
        }
        [HttpPost]
        [Route("UpdateUser")]
        public UserInfo UpdateUser(UserInfo userinfo)
        {
            _userInfoService.Update(userinfo);
            return userinfo;
        }
        [HttpGet]
        [Route("GetPage")]
        public PagedList<UserInfo> GetUserInfosPage([FromQuery] UserParameters userParameters)
        {
            var users = _userInfoService.GetForDepartment(userParameters).ToPagedList(userParameters.PageNumber, userParameters.PageSize);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(users.MetaData));
            return users;
        }
        [HttpDelete]
        [Route("DeleteUser/{userid}")]
        public ActionResult DeleteUser(int userid)
        {
            _userInfoService.Delete(userid);
            return Ok();
        }

    }
}
