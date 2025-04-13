using MediatR;
using spot.Application.Interfaces.Repositories.Orders;
using spot.Application.Wrappers;
using spot.Domain.Orders.Entities;
using spot.Domain.Orders.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace spot.Application.Features.Orders.Queries.GetOrdersByUserId
{
    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, Response<List<Order>>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByUserIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Response<List<Order>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IReadOnlyList<Order> orders;

                if (!string.IsNullOrEmpty(request.ProductId))
                {
                    orders = await _orderRepository.GetOrdersByUserIdAndProductIdAsync(request.UserId, request.ProductId);
                }
                else if (request.OnlyActive)
                {
                    orders = await _orderRepository.GetActiveOrdersByUserIdAsync(request.UserId);
                }
                else
                {
                    orders = await _orderRepository.GetOrdersByUserIdAsync(request.UserId);
                }

                return new Response<List<Order>>(orders.ToList());
            }
            catch (Exception ex)
            {
                return new Response<List<Order>>($"An error occurred while retrieving orders: {ex.Message}");
            }
        }
    }
}