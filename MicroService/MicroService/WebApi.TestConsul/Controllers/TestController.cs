using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.TestConsul.Models;
using WebApi.TestConsul.Utility;

namespace WebApi.TestConsul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public JsonResult Info()
        {
            string url = "http://localhost:5000/api/users/get";

            string result = HttpHelper.InvokeApi(url);

           User user =  System.Text.Json.JsonSerializer.Deserialize<User>(result);

            return new JsonResult(user);
        }
    }
}
