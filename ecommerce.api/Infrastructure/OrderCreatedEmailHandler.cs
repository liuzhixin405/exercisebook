using ECommerce.API.Application;

namespace ECommerce.API.Infrastructure
{
    // 具体事件处理器
    public class OrderCreatedEmailHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEmailHandler> _logger;

        public OrderCreatedEmailHandler(ILogger<OrderCreatedEmailHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(OrderCreatedEvent @event)
        {
            _logger.LogInformation($"Sending confirmation email for order {@event.OrderId}");
            // 发送邮件逻辑
            await Task.CompletedTask;
        }
    }
}
