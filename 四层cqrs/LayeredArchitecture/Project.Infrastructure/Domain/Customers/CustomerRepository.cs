using Microsoft.EntityFrameworkCore;
using Project.Domain.Customers;
using Project.Domain.Customers.Orders;
using Project.Infrastructure.Database;
using Project.Infrastructure.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Domain.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly OrdersContext _;
        public CustomerRepository(OrdersContext context)
        {
            _ = context?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(Customer customer)
        {
            await _.Set<Customer>().AddAsync(customer);
        }

        public Task<Customer> GetByIdAsync(CustomerId id)
        {
            return _.Customers.IncludePaths(CustomerEntityTypeConfiguration.OrderList,CustomerEntityTypeConfiguration.OrderProducts).SingleAsync(x=>x.Id==id);
        }
    }
}
