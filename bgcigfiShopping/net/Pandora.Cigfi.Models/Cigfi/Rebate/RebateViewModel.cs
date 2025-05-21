using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Models.Cigfi.Rebate
{
    public class RebateViewModel
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string InviterLevelDesc { get; set; }
        public decimal RebateRatio { get; set; }
    }
}
