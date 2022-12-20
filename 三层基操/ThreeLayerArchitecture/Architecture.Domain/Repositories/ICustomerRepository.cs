using Architecture.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Repositories
{
    public interface ICustomerRepository:IRepository<Customer>
    {
        Customer FindById(String id);
    }
}
