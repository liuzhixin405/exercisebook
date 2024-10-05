using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Dtos.Orders
{
    public record OrderDto
    {
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
