using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Products
{
    public class ProductId:TypedIdValueBase
    {
        public ProductId(Guid value):base(value)
        {

        }
    }
}
