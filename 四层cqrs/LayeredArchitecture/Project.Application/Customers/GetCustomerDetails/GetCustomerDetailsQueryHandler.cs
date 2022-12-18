using Dapper;
using Project.Application.Configuration.Data;
using Project.Application.Configuration.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Customers.GetCustomerDetails
{
    public class GetCustomerDetailsQueryHandler : IQueryHandler<GetCustomerDetailsQuery, CustomerDetailsDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetCustomerDetailsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        public Task<CustomerDetailsDto> Handle(GetCustomerDetailsQuery request, CancellationToken cancellationToken)
        {
            const string sql = "SELECT " +
                              "[Customer].[Id], " +
                              "[Customer].[Name], " +
                              "[Customer].[Email], " +
                              "[Customer].[WelcomeEmailWasSent] " +
                              "FROM orders.v_Customers AS [Customer] " +
                              "WHERE [Customer].[Id] = @CustomerId ";

            var connection = _sqlConnectionFactory.GetOpenConnection();

            return connection.QuerySingleAsync<CustomerDetailsDto>(sql, new
            {
                request.CustomerId
            });
        }
    }
}
