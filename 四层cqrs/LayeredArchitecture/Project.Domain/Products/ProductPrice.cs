using Project.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Products
{
    public class ProductPrice
    {
        public MoneyValue Value { get; private set; }
        public ProductPrice()
        {

        }
    }
}
