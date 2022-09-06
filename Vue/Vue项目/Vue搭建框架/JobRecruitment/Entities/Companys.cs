using System;
using System.Collections.Generic;

namespace JobRecruitment.Entities
{
    public partial class Companys
    {
        public Companys()
        {
            Jobs = new HashSet<Jobs>();
        }

        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNature { get; set; }
        public string CompanySize { get; set; }
        public string IndustryType { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyIntroduce { get; set; }

        public virtual ICollection<Jobs> Jobs { get; set; }
    }
}
