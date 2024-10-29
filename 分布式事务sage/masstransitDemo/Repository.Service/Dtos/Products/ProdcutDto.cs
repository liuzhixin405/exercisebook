using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Dtos.Products
{
    public record ProductDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
