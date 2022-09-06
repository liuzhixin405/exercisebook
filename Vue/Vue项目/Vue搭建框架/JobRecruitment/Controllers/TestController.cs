using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobRecruitment.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public List<string> Get()
        {
            List<string> strings = new List<string>();
            strings.Add("A");
            strings.Add("B");
            strings.Add("C");
            return strings;
        }
        [HttpGet("{id}")]
        public string Get(int id) { 
            return "AAA"+id;
        }
    }
}