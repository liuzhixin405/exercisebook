using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemoTwo
{
    internal class Order
    {
        public string UserId { get; set; }
        public List<Product> Products = new List<Product>();
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<Product> Proudct => Products;

    }

    internal class Product
    {
        public int Id { get; set; }
    }
}
