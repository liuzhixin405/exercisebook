using System.Data.SqlClient;
using System.Threading.Tasks;
using NUnit.Framework;
using Project.Application.Customers.GetCustomerDetails;
using Project.Application.Customers.IntegrationHandlers;
using Project.Application.Customers.RegisterCustomer;
using Project.Domain.Customers;
using Project.Infrastructure.Processing;
using Project.IntegrationTests.SeedWork;

namespace Project.IntegrationTests.Customers
{
    [TestFixture]
    public class CustomersTests : TestBase
    {
        [Test]
        public async Task RegisterCustomerTest()
        {
            const string email = "newCustomer@mail.com";
            const string name = "Sample Company";
            
            var customer = await CommandsExecutor.Execute(new RegisterCustomerCommand(email, name));
            var customerDetails = await QueriesExecutor.Execute(new GetCustomerDetailsQuery(customer.Id));

            Assert.That(customerDetails, Is.Not.Null);
            Assert.That(customerDetails.Name, Is.EqualTo(name));
            Assert.That(customerDetails.Email, Is.EqualTo(email));

            var connection = new SqlConnection(ConnectionString);
            var messagesList = await OutboxMessagesHelper.GetOutboxMessages(connection);

            Assert.That(messagesList.Count, Is.EqualTo(1));

            var customerRegisteredNotification =
                OutboxMessagesHelper.Deserialize<CustomerRegisteredNotification>(messagesList[0]);

            Assert.That(customerRegisteredNotification.CustomerId, Is.EqualTo(new CustomerId(customer.Id)));
        }
    }
}