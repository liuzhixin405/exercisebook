using Architecture.Domain.Products;
using Architecture.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Services
{
    public interface ICatalogService
    {
        ICollection<Product> GetFeatureProducts(int count);
        AdHocCatalog GetCustomerAdHocCatalog(String customerId);
    }
}
