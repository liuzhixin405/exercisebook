using MediatR;
using spot.Application.Wrappers;
using spot.Domain.Orders.Entities;
using System.Collections.Generic;

namespace spot.Application.Features.Orders.Queries.GetOrdersByUserId
{
    public class GetOrdersByUserIdQuery : IRequest<Response<List<Order>>>
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public bool OnlyActive { get; set; }
    }
}