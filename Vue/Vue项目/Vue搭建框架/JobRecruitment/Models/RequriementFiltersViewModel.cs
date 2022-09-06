using JobRecruitment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobRecruitment.Models
{
    public class RequriementFiltersViewModel
    {
        public string RequriementKey { get; set; }
        public List<string> RequriementItems { get; set; }
        public string Selected { get; set; }
        //public List<string> Educations { get; set; }
        //public List<string> Welfares { get; set; }
        //public List<string> Cities { get; set; }

        private readonly JobRecruitmentContext context;

        public RequriementFiltersViewModel() { }
        public RequriementFiltersViewModel(JobRecruitmentContext context)
        {
            this.context = context;
        }

        public List<RequriementFiltersViewModel> GetRequriement() {
                List<RequriementFiltersViewModel> requriements = 
                new List<RequriementFiltersViewModel>();
            requriements.Add(new RequriementFiltersViewModel {
                RequriementKey = "工作经验",
                RequriementItems = context.Requirements.Select(m => m.Educations)
                .Where(m => !string.IsNullOrEmpty(m)).ToList(),
                Selected = null
            });
            requriements.Add(new RequriementFiltersViewModel
            {
                RequriementKey = "工作福利",
                RequriementItems = context.Requirements.Select(m => m.Welfares)
                .Where(m => !string.IsNullOrEmpty(m)).ToList(),
                Selected = null
            });
            requriements.Add(new RequriementFiltersViewModel
            {
                RequriementKey = "所在城市",
                RequriementItems = context.Cities.Select(m => m.City).ToList(),
                Selected = null
            });
            return requriements;
        }
    }
}
