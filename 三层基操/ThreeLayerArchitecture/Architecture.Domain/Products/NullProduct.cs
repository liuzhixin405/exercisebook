using Architecture.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Products
{
    public class NullProduct:Product
    {
        public static NullProduct Instance = new NullProduct();
        public NullProduct():base(0,"",Money.Zero,0)
        {

        }
    }
}
