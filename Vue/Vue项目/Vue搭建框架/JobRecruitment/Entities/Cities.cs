using System;
using System.Collections.Generic;

namespace JobRecruitment.Entities
{
    public partial class Cities
    {
        public Cities()
        {
            Jobs = new HashSet<Jobs>();
        }

        public int Id { get; set; }
        public string City { get; set; }
        public int? AdministrativeLevel { get; set; }

        public virtual ICollection<Jobs> Jobs { get; set; }
    }
}
