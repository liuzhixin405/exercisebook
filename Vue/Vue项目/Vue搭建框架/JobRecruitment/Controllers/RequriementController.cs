using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobRecruitment.Entities;
using JobRecruitment.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobRecruitment.Controllers
{
    [EnableCors("any")]
    [Route("[controller]")]
    [ApiController]
    public class RequriementController : ControllerBase
    {
        public JobRecruitmentContext Context { get; }

        public RequriementController(JobRecruitmentContext context)
        {
            Context = context;
        }

        // GET: api/Requriement
        [HttpGet]
        public List<RequriementFiltersViewModel> Get()
        {
            return new RequriementFiltersViewModel(Context).GetRequriement();
        }

        // GET: api/Requriement/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Requriement
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Requriement/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
