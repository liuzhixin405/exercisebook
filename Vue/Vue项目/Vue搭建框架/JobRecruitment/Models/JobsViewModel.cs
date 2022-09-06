using JobRecruitment.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobRecruitment.Models
{
    public class JobsViewModel
    {
        private readonly JobRecruitmentContext context;

        public int Id { get; set; }
        public string JobName { get; set; }
        public string JobPay { get; set; }
        public string Welfare { get; set; }
        public string Education { get; set; }
        public string WorkExperience { get; set; }
        public int? WorkPlace { get; set; }
        public string WorkArea { get; set; }
        public DateTime? PublishTime { get; set; }
        public string PositionInfo { get; set; }
        public int? CompanyId { get; set; }

        public Companys Company { get; set; }
        public Cities City { get; set; }

        private readonly JobRecruitmentContext _context;
        public JobsViewModel()
        {
        }
        public JobsViewModel(JobRecruitmentContext context)
        {
            _context = context;
        }
        public List<JobsViewModel> GetJobs()
        {
            ////不推荐，缺陷明显，会寻循环引用
            //var jobs = context.Jobs.Include(m => m.Company).Include(m => m.City).ToList();
            ////方案一
            //JsonSerializerSettings setting = new JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    Formatting = Formatting.None
            //};
            //var json = JsonConvert.SerializeObject(jobs, setting);

            ////方案二（可用）
            //jobs.ForEach(m => m.Company.Jobs = null);
            //jobs.ForEach(m => m.City.Jobs = null);
            ////可用方案
            //context.Jobs.Join(context.Companys, j => j.CompanyId, c => c.Id, (j, c) =>
            //new JobsViewModel
            //{
            //    Id = j.Id,
            //    JobName = j.JobName,
            //    JobPay = j.JobPay,
            //    Welfare = j.Welfare,
            //    Education = j.Education,
            //    WorkExperience = j.WorkExperience,
            //    WorkPlace = j.WorkPlace,
            //    WorkArea = j.WorkArea,
            //    PublishTime = j.PublishTime,
            //    PositionInfo = j.PositionInfo,
            //    CompanyId = j.CompanyId,
            //    Company = c,
            //}).Join(context.Cities, j => j.WorkPlace, c => c.Id, (j, c) => new JobsViewModel
            //{
            //    Id = j.Id,
            //    JobName = j.JobName,
            //    JobPay = j.JobPay,
            //    Welfare = j.Welfare,
            //    Education = j.Education,
            //    WorkExperience = j.WorkExperience,
            //    WorkPlace = j.WorkPlace,
            //    WorkArea = j.WorkArea,
            //    PublishTime = j.PublishTime,
            //    PositionInfo = j.PositionInfo,
            //    CompanyId = j.CompanyId,
            //    Company = j.Company,
            //    City = j.City

            //});
            var jobsJoin = from j in _context.Jobs
                       join cp in _context.Companys on j.CompanyId equals cp.Id
                       join ct in _context.Cities on j.WorkPlace equals ct.Id
                       select new JobsViewModel
                       {
                           Id = j.Id,
                           JobName = j.JobName,
                           JobPay = j.JobPay,
                           Welfare = j.Welfare,
                           Education = j.Education,
                           WorkExperience = j.WorkExperience,
                           WorkPlace = j.WorkPlace,
                           WorkArea = j.WorkArea,
                           PublishTime = j.PublishTime,
                           PositionInfo = j.PositionInfo,
                           CompanyId = j.CompanyId,
                           Company = cp,
                           City = ct,
                       };
            return jobsJoin.ToList();
        }
        public Jobs GetJobById(int id)
        {
            var jobs = _context.Jobs.Where(m => m.Id == id)
                .Include(m => m.Company)
                .Include(m => m.City).ToList();
            jobs.ForEach(m => m.Company.Jobs = null);
            jobs.ForEach(m => m.City.Jobs = null);
            return jobs.FirstOrDefault();
        }
    }
}
