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
    public class JobController : ControllerBase
    {
        public JobRecruitmentContext Context { get; }

        public JobController(JobRecruitmentContext context)
        {
            Context = context;
        }
        // GET: api/Job
        [HttpGet]
        public IEnumerable<JobsViewModel> Get()
        {
            JobsViewModel jobsViewModel = new JobsViewModel(Context);
            return jobsViewModel.GetJobs(); ;
        }

        // GET: api/Job/5
        [HttpGet("{id}")]
        public Jobs Get(int id)
        {
            JobsViewModel jobsViewModel = new JobsViewModel(Context);
            return jobsViewModel.GetJobById(id); ;
        }

        //// POST: api/Job
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Job/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
