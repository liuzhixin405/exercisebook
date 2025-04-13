using MediatR;
using spot.Application.Wrappers;
using spot.Domain.Orders.Entities;

namespace spot.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Response<Order>>
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public decimal Price { get; set; }
        public decimal Size { get; set; }
    }
}