using System;
using MediatR;
using Project.Application.Configuration.Commands;

namespace Project.Application.Orders.RemoveCustomerOrder
{
    public class RemoveCustomerOrderCommand : CommandBase
    {
        public Guid CustomerId { get; }

        public Guid OrderId { get; }

        public RemoveCustomerOrderCommand(
            Guid customerId,
            Guid orderId)
        {
            this.CustomerId = customerId;
            this.OrderId = orderId;
        }
    }
}