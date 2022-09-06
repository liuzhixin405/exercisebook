using System;
using System.Collections.Generic;

namespace JobRecruitment.Entities
{
    public partial class Jobs
    {
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

        public virtual Companys Company { get; set; }
        public virtual Cities City { get; set; }
    }
}
