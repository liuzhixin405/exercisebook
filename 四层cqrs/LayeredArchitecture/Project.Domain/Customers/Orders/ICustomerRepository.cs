using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Orders
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(CustomerId id);
        Task AddAsync(Customer customer);
    }
}
