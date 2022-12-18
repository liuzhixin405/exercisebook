using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Application.Configuration.Commands;
using Project.Domain.Customers;
using Project.Domain.Customers.Orders;

namespace Project.Application.Orders.RemoveCustomerOrder
{
    public class RemoveCustomerOrderCommandHandler : ICommandHandler<RemoveCustomerOrderCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        public RemoveCustomerOrderCommandHandler(ICustomerRepository customerRepository)
        {
            this._customerRepository = customerRepository;
        }

        public async Task<Unit> Handle(RemoveCustomerOrderCommand request, CancellationToken cancellationToken)
        {
            var customer = await this._customerRepository.GetByIdAsync(new CustomerId(request.CustomerId));

            customer.RemoveOrder(new OrderId(request.OrderId));

            return Unit.Value;
        }
    }
}