using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.QueryStack.Model
{
    public class FixedPriceJobOrder : JobOrder
    {
        public decimal Price { get; set; }
        public DateTime DueDate { get; set; }
    }
}
