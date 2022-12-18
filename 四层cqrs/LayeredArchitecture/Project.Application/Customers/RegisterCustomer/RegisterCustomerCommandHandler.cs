using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Application.Configuration.Commands;
using Project.Domain.Customers;
using Project.Domain.Customers.Orders;
using Project.Domain.SeedWork;

namespace Project.Application.Customers.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : ICommandHandler<RegisterCustomerCommand, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerUniquenessChecker _customerUniquenessChecker;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCustomerCommandHandler(
            ICustomerRepository customerRepository, 
            ICustomerUniquenessChecker customerUniquenessChecker, 
            IUnitOfWork unitOfWork)
        {
            this._customerRepository = customerRepository;
            _customerUniquenessChecker = customerUniquenessChecker;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerDto> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = Customer.CreateRegistered(request.Email, request.Name, this._customerUniquenessChecker);

            await this._customerRepository.AddAsync(customer);

            await this._unitOfWork.CommitAsync(cancellationToken);

            return new CustomerDto { Id = customer.Id.Value };
        }
    }
}