using Microsoft.AspNetCore.Http.HttpResults;
using Repository.Service.Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Dtos
{
    public class CreateDto:ICreateTransaction
    {
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }  
    }
}
