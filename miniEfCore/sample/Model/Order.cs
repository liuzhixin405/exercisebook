using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample.Model
{
    public class Orders
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double Discount { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
