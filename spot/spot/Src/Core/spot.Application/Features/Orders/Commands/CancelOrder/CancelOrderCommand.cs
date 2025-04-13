using MediatR;
using spot.Application.Wrappers;

namespace spot.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<Response<bool>>
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
    }
}