using MediatR;
using spot.Application.Interfaces;
using spot.Application.Interfaces.Repositories;
using spot.Application.Interfaces.Repositories.Accounts;
using spot.Application.Interfaces.Repositories.Orders;
using spot.Application.Wrappers;
using spot.Domain.Orders.Entities;
using spot.Domain.Orders.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace spot.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<Order>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate product exists
                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return new Response<Order>($"Product with ID {request.ProductId} not found");
                }

                // Validate order type
                if (request.Type != OrderType.Limit && request.Type != OrderType.Market)
                {
                    return new Response<Order>($"Invalid order type: {request.Type}");
                }

                // Validate order side
                if (request.Side != OrderSide.Buy && request.Side != OrderSide.Sell)
                {
                    return new Response<Order>($"Invalid order side: {request.Side}");
                }

                // Check if size is within limits
                if (request.Size < product.BaseMinSize || (product.BaseMaxSize > 0 && request.Size > product.BaseMaxSize))
                {
                    return new Response<Order>($"Order size must be between {product.BaseMinSize} and {product.BaseMaxSize}");
                }

                // For limit orders, check if price is valid
                if (request.Type == OrderType.Limit && request.Price <= 0)
                {
                    return new Response<Order>("Price must be greater than zero for limit orders");
                }

                // Check if user has sufficient funds
                string currencyToCheck = request.Side == OrderSide.Buy ? product.QuoteCurrency : product.BaseCurrency;
                var account = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(request.UserId, currencyToCheck);
                
                if (account == null)
                {
                    return new Response<Order>($"No account found for user {request.UserId} with currency {currencyToCheck}");
                }

                decimal requiredFunds;
                if (request.Side == OrderSide.Buy)
                {
                    // For buy orders, we need to check if the user has enough quote currency
                    requiredFunds = request.Price * request.Size;
                }
                else
                {
                    // For sell orders, we need to check if the user has enough base currency
                    requiredFunds = request.Size;
                }

                if (account.Available < requiredFunds)
                {
                    return new Response<Order>($"Insufficient funds. Required: {requiredFunds} {currencyToCheck}, Available: {account.Available} {currencyToCheck}");
                }

                // Hold the funds
                await _accountRepository.UpdateBalanceAsync(
                    request.UserId,
                    currencyToCheck,
                    -requiredFunds,  // Reduce available
                    requiredFunds    // Increase hold
                );

                // Create the order
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedAt = now,
                    UpdatedAt = now,
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    Type = request.Type,
                    Side = request.Side,
                    Price = request.Price,
                    Size = request.Size,
                    FilledSize = 0,
                    FilledFunds = 0,
                    Status = OrderStatus.Open
                };

                var createdOrder = await _orderRepository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new Response<Order>(createdOrder);
            }
            catch (Exception ex)
            {
                return new Response<Order>($"An error occurred while creating the order: {ex.Message}");
            }
        }
    }
}