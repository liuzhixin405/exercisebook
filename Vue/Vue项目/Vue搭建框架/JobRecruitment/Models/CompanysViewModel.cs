using JobRecruitment.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobRecruitment.Models
{
    public class CompanysViewModel
    {
        private readonly JobRecruitmentContext _context;

        public CompanysViewModel(JobRecruitmentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public List<Companys> GetCompanys()
        {
            var companys = _context.Companys.Include(m=>m.Jobs).ToList();
            companys.ToList().ForEach(m => m.Jobs.ToList()
            .ForEach(n => n.Company = null));
            return companys;
        }
        public Companys GetCompanyById(int id) {
            var company = _context.Companys.Include(m => m.Jobs)
                .First(m => m.Id == id);
            company.Jobs.ToList().ForEach(m => m.Company = null);
            company.Jobs.ToList().ForEach(m => m.City.Jobs = null);
          return company;
        }

    }
}
