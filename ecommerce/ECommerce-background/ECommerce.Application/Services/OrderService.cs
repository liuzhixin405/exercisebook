using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Events;
using ECommerce.Domain.Models;
using ECommerce.Core.EventBus;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IPaymentService _paymentService;
        private readonly IInventoryService _inventoryService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IAddressRepository addressRepository,
            IPaymentService paymentService,
            IInventoryService inventoryService,
            IEventBus eventBus,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _paymentService = paymentService;
            _inventoryService = inventoryService;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto createOrderDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");
          
            // 验证地址
            Address? address = null;
            if (createOrderDto.AddressId.HasValue)
            {
                address = await _addressRepository.GetByIdAsync(createOrderDto.AddressId.Value);
                if (address == null || address.UserId != userId)
                    throw new ArgumentException("指定的地址不存在或无权限访问，请先设置默认地址");
            }
            else
            {
                // 如果没有提供地址ID，尝试获取用户的默认地址
                address = await _addressRepository.GetDefaultByUserIdAsync(userId);
                if (address == null)
                    throw new ArgumentException("请先设置默认收货地址");
            }

            // 验证库存并锁定
            var inventoryUpdates = new List<InventoryUpdate>();
            foreach (var itemDto in createOrderDto.Items)
            {
                var stockCheck = await _inventoryService.CheckStockAsync(itemDto.ProductId, itemDto.Quantity);
                if (!stockCheck.IsAvailable)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {itemDto.ProductId}. Available: {stockCheck.AvailableStock}, Requested: {itemDto.Quantity}");
                }

                // 锁定库存
                var lockResult = await _inventoryService.LockStockAsync(itemDto.ProductId, itemDto.Quantity, Guid.Empty);
                if (!lockResult.Success)
                {
                    throw new InvalidOperationException($"Failed to lock stock for product {itemDto.ProductId}: {lockResult.Message}");
                }

                inventoryUpdates.Add(new InventoryUpdate
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    OperationType = InventoryOperationType.Lock,
                    Reason = "Order creation",
                    Notes = "Stock locked during order creation"
                });
            }

            var order = new Order
            {
                UserId = userId,
                CustomerName = string.IsNullOrEmpty(createOrderDto.CustomerName) ? address.Name : createOrderDto.CustomerName,
                PhoneNumber = string.IsNullOrEmpty(createOrderDto.PhoneNumber) ? address.Phone : createOrderDto.PhoneNumber,
                ShippingAddress = string.IsNullOrEmpty(createOrderDto.ShippingAddress) ? address.FullAddress : createOrderDto.ShippingAddress,
                PaymentMethod = createOrderDto.PaymentMethod,
                Notes = createOrderDto.Notes,
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            foreach (var itemDto in createOrderDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product with id {itemDto.ProductId} not found");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = product.Price
                };

                order.Items.Add(orderItem);
                totalAmount += product.Price * itemDto.Quantity;
            }

            order.TotalAmount = totalAmount;

            var createdOrder = await _orderRepository.AddAsync(order);
            _logger.LogInformation("Created order: {OrderId} for user: {UserId}", createdOrder.Id, userId);

            // 发布订单创建事件
            try
            {
                var orderCreatedEvent = new OrderCreatedEvent(createdOrder);

                await _eventBus.PublishAsync(orderCreatedEvent);

                // 发布库存锁定事件
                foreach (var item in createdOrder.Items)
                {
                    var stockLockedEvent = new StockLockedEvent(
                        item.ProductId,
                        item.Product?.Name ?? "Unknown Product",
                        item.Quantity,
                        createdOrder.Id);

                    await _eventBus.PublishAsync(stockLockedEvent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish events for order {OrderId}", createdOrder.Id);
                // 事件发布失败不应该影响订单创建，只记录日志
            }

            return MapToDto(createdOrder);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto updateOrderStatusDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new ArgumentException($"Order with id {id} not found");

            var oldStatus = order.Status;
            order.Status = updateOrderStatusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(updateOrderStatusDto.TrackingNumber))
                order.TrackingNumber = updateOrderStatusDto.TrackingNumber;

            if (!string.IsNullOrEmpty(updateOrderStatusDto.Notes))
                order.Notes = updateOrderStatusDto.Notes;

            var updatedOrder = await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Updated order status: {OrderId} to {Status}", id, updateOrderStatusDto.Status);

            // 发布订单状态变更事件
            try
            {
                var statusChangedEvent = new OrderStatusChangedEvent(order.Id, order.UserId, oldStatus, order.Status);
                await _eventBus.PublishAsync(statusChangedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish order status changed event for order {OrderId}", order.Id);
            }

            return MapToDto(updatedOrder);
        }

        public async Task<bool> CancelOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending)
                return false;

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;

            // 释放锁定的库存
            await ReleaseOrderStock(order);

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Cancelled order: {OrderId}", id);

            // 发布订单取消事件
            try
            {
                var orderCancelledEvent = new OrderCancelledEvent(
                    order.Id,
                    order.UserId,
                    "Order cancelled by user or system",
                    order.Status);

                await _eventBus.PublishAsync(orderCancelledEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish order cancelled event for order {OrderId}", order.Id);
            }

            return true;
        }

        public async Task<bool> ProcessPaymentAsync(PaymentDto paymentDto)
        {
            var order = await _orderRepository.GetByIdAsync(paymentDto.OrderId);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending)
                return false;

            if (paymentDto.Amount != order.TotalAmount)
                return false;

            // 创建支付请求
            var paymentRequest = new PaymentRequest
            {
                OrderId = paymentDto.OrderId,
                PaymentMethod = paymentDto.PaymentMethod,
                Amount = paymentDto.Amount,
                Currency = "CNY",
                Description = $"Payment for order {paymentDto.OrderId}",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", paymentDto.OrderId.ToString() },
                    { "UserId", order.UserId.ToString() }
                }
            };

            // 处理支付
            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);
            if (!paymentResult.Success)
            {
                _logger.LogWarning("Payment failed for order: {OrderId}, Error: {Error}", 
                    paymentDto.OrderId, paymentResult.Message);

                // 发布支付失败事件
                try
                {
                    var paymentFailedEvent = new PaymentFailedEvent(
                        paymentResult.PaymentId ?? string.Empty,
                        order.Id,
                        order.UserId,
                        paymentDto.Amount,
                        paymentDto.PaymentMethod,
                        paymentResult.Message);
                    await _eventBus.PublishAsync(paymentFailedEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish payment failed event for order {OrderId}", order.Id);
                }

                return false;
            }

            // 支付成功，更新订单状态
            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            order.PaymentMethod = paymentDto.PaymentMethod;

            // 扣减实际库存
            await DeductOrderStock(order);

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Processed payment for order: {OrderId}, PaymentId: {PaymentId}", 
                paymentDto.OrderId, paymentResult.PaymentId);

            // 发布支付成功事件
            try
            {
                var orderPaidEvent = new OrderPaidEvent(
                    order.Id,
                    order.UserId,
                    paymentResult.PaymentId,
                    paymentDto.Amount,
                    paymentDto.PaymentMethod);

                await _eventBus.PublishAsync(orderPaidEvent);

                var paymentSucceededEvent = new PaymentSucceededEvent(
                    paymentResult.PaymentId,
                    order.Id,
                    order.UserId,
                    paymentDto.Amount,
                    paymentDto.PaymentMethod);

                await _eventBus.PublishAsync(paymentSucceededEvent);

                var paymentProcessedEvent = new PaymentProcessedEvent(
                    paymentResult.PaymentId,
                    order.Id,
                    order.UserId,
                    paymentDto.Amount,
                    paymentDto.PaymentMethod,
                    true);

                await _eventBus.PublishAsync(paymentProcessedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish payment events for order {OrderId}", order.Id);
            }

            return true;
        }

        // 网关已成功后，仅进行订单侧确认与事件发布
        public async Task<bool> FinalizePaymentAsync(Guid orderId, string paymentMethod, decimal amount, string paymentId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending)
                return false;

            if (amount != order.TotalAmount)
                return false;

            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            order.PaymentMethod = paymentMethod;

            await DeductOrderStock(order);
            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Finalized payment for order: {OrderId}, PaymentId: {PaymentId}", orderId, paymentId);

            try
            {
                await _eventBus.PublishAsync(new OrderPaidEvent(order.Id, order.UserId, paymentId, amount, paymentMethod));
                await _eventBus.PublishAsync(new PaymentSucceededEvent(paymentId, order.Id, order.UserId, amount, paymentMethod));
                await _eventBus.PublishAsync(new PaymentProcessedEvent(paymentId, order.Id, order.UserId, amount, paymentMethod, true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish payment events for order {OrderId}", order.Id);
            }

            return true;
        }

        public async Task<bool> ConfirmOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Paid)
                return false;

            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Confirmed order: {OrderId}", orderId);

            // 发布订单状态变更事件
            try
            {
                var statusChangedEvent = new OrderStatusChangedEvent(
                    orderId,
                    order.UserId,
                    OrderStatus.Paid,
                    OrderStatus.Confirmed,
                    "Order confirmed after payment");

                await _eventBus.PublishAsync(statusChangedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish order status changed event for order {OrderId}", orderId);
            }

            return true;
        }

        public async Task<bool> ShipOrderAsync(Guid id, string trackingNumber)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Confirmed)
                return false;

            order.Status = OrderStatus.Shipped;
            order.ShippedAt = DateTime.UtcNow;
            order.TrackingNumber = trackingNumber;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Shipped order: {OrderId} with tracking: {TrackingNumber}", id, trackingNumber);
            return true;
        }

        public async Task<bool> DeliverOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Shipped)
                return false;

            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Delivered order: {OrderId}", id);
            return true;
        }

        public async Task<bool> CancelExpiredOrdersAsync()
        {
            // 取消超过30分钟未支付的订单
            var expirationTime = TimeSpan.FromMinutes(30);
            var expiredOrders = await _orderRepository.GetExpiredPendingOrdersAsync(expirationTime);

            foreach (var order in expiredOrders)
            {
                order.Status = OrderStatus.Cancelled;
                order.CancelledAt = DateTime.UtcNow;
                
                // 释放锁定的库存
                await ReleaseOrderStock(order);
                
                await _orderRepository.UpdateAsync(order);
                _logger.LogInformation("Cancelled expired order: {OrderId}", order.Id);
            }

            return true;
        }

        /// <summary>
        /// 释放订单相关的库存锁定
        /// </summary>
        private async Task ReleaseOrderStock(Order order)
        {
            foreach (var item in order.Items)
            {
                var releaseResult = await _inventoryService.ReleaseLockedStockAsync(
                    item.ProductId, item.Quantity, order.Id);
                
                if (!releaseResult.Success)
                {
                    _logger.LogWarning("Failed to release locked stock for product: {ProductId}, Order: {OrderId}, Error: {Error}", 
                        item.ProductId, order.Id, releaseResult.Message);
                }
            }
        }

        /// <summary>
        /// 扣减订单相关的实际库存
        /// </summary>
        private async Task DeductOrderStock(Order order)
        {
            foreach (var item in order.Items)
            {
                var deductResult = await _inventoryService.DeductStockAsync(item.ProductId, item.Quantity);
                
                if (!deductResult.Success)
                {
                    _logger.LogWarning("Failed to deduct stock for product: {ProductId}, Order: {OrderId}, Error: {Error}", 
                        item.ProductId, order.Id, deductResult.Message);
                }
            }
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                PhoneNumber = order.PhoneNumber,
                CustomerName = order.CustomerName,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                PaidAt = order.PaidAt,
                ShippedAt = order.ShippedAt,
                DeliveredAt = order.DeliveredAt,
                CancelledAt = order.CancelledAt,
                PaymentMethod = order.PaymentMethod,
                TrackingNumber = order.TrackingNumber,
                Notes = order.Notes,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };
        }

        public async Task<bool> CompleteOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Delivered)
                return false;

            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Completed order: {OrderId}", orderId);

            // 发布订单完成事件
            try
            {
                var orderCompletedEvent = new OrderStatusChangedEvent(
                    orderId,
                    order.UserId,
                    OrderStatus.Delivered,
                    OrderStatus.Completed,
                    "Order completed by customer");

                await _eventBus.PublishAsync(orderCompletedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish order completed event for order {OrderId}", orderId);
            }

            return true;
        }
    }
}