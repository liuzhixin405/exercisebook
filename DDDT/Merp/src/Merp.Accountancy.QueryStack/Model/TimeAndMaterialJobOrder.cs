using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.QueryStack.Model
{
    public class TimeAndMaterialJobOrder : JobOrder
    {
        public decimal? Value { get; set; }
        public DateTime? DateOfExpiration { get; set; }

    }
}
