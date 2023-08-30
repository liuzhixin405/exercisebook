using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBuyStuff.Domain.Customers;
using IBuyStuff.Domain.Products;

namespace IBuyStuff.Domain.Services.DTO
{
    public class AdHocCatalog
    {
        public AdHocCatalog()
        {
            Customer = MissingCustomer.Instance;
            Products = new List<Product>();
        }
        public Customer Customer { get; set; }
        public IList<Product> Products { get; set; }
    }
}
