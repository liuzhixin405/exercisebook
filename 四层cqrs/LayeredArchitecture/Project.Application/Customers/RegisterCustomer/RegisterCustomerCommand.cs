using MediatR;
using Project.Application.Configuration.Commands;

namespace Project.Application.Customers.RegisterCustomer
{
    public class RegisterCustomerCommand : CommandBase<CustomerDto>
    {
        public string Email { get; }

        public string Name { get; }

        public RegisterCustomerCommand(string email, string name)
        {
            this.Email = email;
            this.Name = name;
        }      
    }
}