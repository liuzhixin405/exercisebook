using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AuthenticationCenter.Interface;
using AuthenticationCenter.Models;
using AuthenticationCenter.Utility.RSA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AuthenticationCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IJwtService _iJwtService;
        private readonly IConfiguration _iConfiguration;
        public AuthenticationController(IJwtService iJwtService, IConfiguration iConfiguration)
        {
            _iJwtService = iJwtService;
            _iConfiguration = iConfiguration;
        }
        [Route("GetKey")]
        [HttpGet]
        public string GetKey()
        {
            string keyDir = Directory.GetCurrentDirectory();
            if (RsaHelper.TryGetKeyParameters(keyDir, false, out RSAParameters keyParams) == false)
            {
                keyParams = RsaHelper.GenerateAndSaveKey(keyDir, false);
            }

            return JsonConvert.SerializeObject(keyParams);
        }
        [Route("Login")]
        [HttpGet]
        public string Login(string name,string password)
        {
            if("admin".Equals(name) && "123456".Equals(password))
            {
                CurrentUserModel currentUser = new CurrentUserModel()
                {
                    Id = 123,
                    Account = "www.12306.com",
                    EMail = "164910441@qq.com",
                    Mobile = "13000000000",
                    Sex = 1,
                    Age = 33,
                    Name = "lzx",
                    Role = "Admin"
                };
                string token = _iJwtService.GetToken(currentUser);

                return JsonConvert.SerializeObject(new
                {
                    result = true,
                    token
                    
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    result = true,
                    token=""

                });
            }
           
        }
    }
}
