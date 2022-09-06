using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Model
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        /// <summary>
        /// 信用
        /// </summary>
        public decimal Credit { get; set; }
    }
}
