using MediatR;
using spot.Application.Interfaces;
using spot.Application.Interfaces.Repositories.Accounts;
using spot.Application.Interfaces.Repositories.Orders;
using spot.Application.Interfaces.Repositories;
using spot.Application.Wrappers;
using spot.Domain.Orders.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace spot.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Response<bool>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            IAccountRepository accountRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the order
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    return new Response<bool>($"Order with ID {request.OrderId} not found");
                }

                // Check if the order belongs to the user
                if (order.UserId != request.UserId)
                {
                    return new Response<bool>("You are not authorized to cancel this order");
                }

                // Check if the order can be canceled
                if (order.Status != OrderStatus.Open && order.Status != OrderStatus.Pending)
                {
                    return new Response<bool>($"Cannot cancel order with status {order.Status}");
                }

                // Calculate remaining funds to be released
                decimal remainingSize = order.Size - order.FilledSize;
                decimal fundsToRelease;

                // Determine the currency and amount to release
                var product = await _productRepository.GetByIdAsync(order.ProductId);
                string currency;

                if (order.Side == OrderSide.Buy)
                {
                    // For buy orders, release quote currency (e.g., USD)
                    currency = product.QuoteCurrency;
                    fundsToRelease = remainingSize * order.Price;
                }
                else
                {
                    // For sell orders, release base currency (e.g., BTC)
                    currency = product.BaseCurrency;
                    fundsToRelease = remainingSize;
                }

                // Release the funds from hold
                await _accountRepository.UpdateBalanceAsync(
                    order.UserId,
                    currency,
                    fundsToRelease,   // Increase available
                    -fundsToRelease    // Decrease hold
                );

                // Update order status
                await _orderRepository.UpdateOrderStatusAsync(order.Id, OrderStatus.Canceled);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new Response<bool>(true, "Order canceled successfully");
            }
            catch (Exception ex)
            {
                return new Response<bool>($"An error occurred while canceling the order: {ex.Message}");
            }
        }
    }
}