using Dapper;
using Project.Application.Configuration.Data;
using Project.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Customers.DomainServices
{
    public class CustomerUniquenessChecker:ICustomerUniquenessChecker
    {
        private readonly ISqlConnectionFactory sqlConnectionFactory;
        public CustomerUniquenessChecker(ISqlConnectionFactory sqlConnectionFactory)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
        }

        public bool IsUnique(String customerEmail)
        {
            var connection = sqlConnectionFactory.GetOpenConnection();
            const string sql = "SELECT TOP 1 1" +
                             "FROM [orders].[Customers] AS [Customer] " +
                             "WHERE [Customer].[Email] = @Email";

            var customerNumer = connection.QuerySingleOrDefault<int?>(sql, new
            {
                Email = customerEmail
            });
            return !customerNumer.HasValue;
        }
    }
}
