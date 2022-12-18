using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Domain.Customers.Orders.Events;
using Project.Domain.Payments;

namespace Project.Application.Orders.PlaceCustomerOrder
{
    public class OrderPlacedDomainEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly IPaymentRepository _paymentRepository;

        public OrderPlacedDomainEventHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
        {
            var newPayment = new Payment(notification.OrderId);

            await this._paymentRepository.AddAsync(newPayment);
        }
    }
}