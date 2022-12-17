using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Orders
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }



        public string Name { get; set; }

        public ProductDto()
        {

        }

        public ProductDto(Guid id, int quantity)
        {
            this.Id = id;
            this.Quantity = quantity;
        }
    }
}
