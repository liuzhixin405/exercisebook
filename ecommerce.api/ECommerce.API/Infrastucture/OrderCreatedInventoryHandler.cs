using ECommerce.API.Application;

namespace ECommerce.API.Infrastucture
{
    public class OrderCreatedInventoryHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedInventoryHandler> _logger;

        public OrderCreatedInventoryHandler(ILogger<OrderCreatedInventoryHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(OrderCreatedEvent @event)
        {
            _logger.LogInformation($"Updating inventory for order {@event.OrderId}");
            // 库存更新逻辑
            await Task.CompletedTask;
        }
    }
}
