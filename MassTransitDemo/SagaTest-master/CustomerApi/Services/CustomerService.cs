using CustomerApi.Dtos;
using CustomerApi.Infrastructure;
using CustomerApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerDbContext dbContext;

        public CustomerService(CustomerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        /// <summary>
        /// 增加信用
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task AddCredit(decimal credit, int customerId)
        {
            if (credit <= 0)
            {
                throw new Exception($"Credit value must be a positive value!");
            }
            var customer = dbContext.Customers.FirstOrDefault(x => x.Id == customerId);
            if (customer == null)
            {
                throw new Exception($"Customer wit {customerId} identifier not found!");
            }

            customer.Credit += credit;
            return dbContext.SaveChangesAsync();
        }

        public async Task<int> CreateCustomer(CreateCustomerDto customerDto)
        {
            var customer = new Customer();
            customer.Credit = customerDto.Credit;
            customer.FullName = customerDto.FullName;
            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();
            return customer.Id;
        }

        /// <summary>
        /// 减信用
        /// </summary>
        /// <param name="credit"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task WithdrawCredit(decimal credit, int customerId)
        {
            if (credit <= 0)
            {
                throw new Exception($"Credit value must be a positive value!");
            }            
            var customer = dbContext.Customers.FirstOrDefault(x => x.Id == customerId);
            if (customer == null)
            {
                throw new Exception($"Customer with {customerId} identifier not found!");
            }
            if (customer.Credit < credit)
            {
                throw new Exception($"Customer {customerId} credit is less than {credit}!");
            }
            customer.Credit -= credit;
            return dbContext.SaveChangesAsync();
        }
    }
}
