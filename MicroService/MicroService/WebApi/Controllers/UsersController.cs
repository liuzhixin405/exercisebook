using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgilityFramework.ConsulClientExtend.ConsulClientExtend;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region DataInit
        private List<User> _UserList = new List<User>()
        {
            new User()
            {
                Id=1,
                Account="Administrator",
                Email="164910441@qq.com",
                Name="Test001",
                Password="1234567890",
                LoginTime=DateTime.Now,
                Role="Admin"
            },
             new User()
            {
                Id=1,
                Account="Apple",
                Email="164910441@qq.com",
                Name="Apple",
                Password="1234567890",
                LoginTime=DateTime.Now,
                Role="Admin"
            },
              new User()
            {
                Id=1,
                Account="Cole",
                Email="164910441@qq.com",
                Name="Cole",
                Password="1234567890",
                LoginTime=DateTime.Now,
                Role="Admin"
            },
        };
        #endregion

        private readonly ILogger<UsersController> _logger;
        private readonly IConfiguration _IConfiguration;
        private readonly IConsulIDistributed _IConsulIDistributed;
        public UsersController(ILogger<UsersController> logger
            , IConsulIDistributed consulIDistributed
            , IConfiguration configuration)
        {
            this._logger = logger;
            this._IConfiguration = configuration;
            this._IConsulIDistributed = consulIDistributed;
        }
        [HttpGet]
        [Route("All")]
        public IEnumerable<User> Get()
        {
            this._logger.LogInformation($"This is UserController-Get {this._IConfiguration["port"]}");
            return this._UserList.Select(u => new User()
            {
                Id = u.Id,
                Account = u.Account,
                Name = u.Name,
                Role = $"{ this._IConfiguration["ip"]}{ this._IConfiguration["port"]}",
                Email = u.Email,
                LoginTime = u.LoginTime,
                Password = u.Password
            });
        }

        [HttpGet]
        [Route("Get")]
        public JsonResult Get(int id)
        {
            User result = this._UserList.FirstOrDefault(u => u.Id == id);
            return new JsonResult(result);
        }
    }
}
