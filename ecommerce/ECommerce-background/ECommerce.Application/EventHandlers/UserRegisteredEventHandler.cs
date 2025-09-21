using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 用户注册事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly ILogger<UserRegisteredEventHandler> _logger;

        public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("UserRegisteredEventHandler: Processing user registration for user {UserId}", domainEvent.UserId);

                // 核心业务逻辑：用户注册确认
                await ProcessUserRegistrationAsync(domainEvent, cancellationToken);

                _logger.LogInformation("UserRegisteredEventHandler: Successfully processed user registration for user {UserId}", domainEvent.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserRegisteredEventHandler: Error processing user registration for user {UserId}", domainEvent.UserId);
                return false;
            }
        }

        /// <summary>
        /// 处理用户注册的核心业务逻辑
        /// </summary>
        private async Task ProcessUserRegistrationAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1. 记录用户注册日志
            _logger.LogInformation("User {UserId} registered with email {Email} at {Timestamp}", 
                domainEvent.UserId, domainEvent.Email, domainEvent.OccurredOn);

            // 2. 可以在这里添加其他核心业务逻辑
            // 比如：初始化用户购物车、设置默认偏好等
            
            // 3. 模拟异步处理
            await Task.Delay(50, cancellationToken);
            
            _logger.LogInformation("User registration processing completed for user {UserId}", domainEvent.UserId);
        }
    }
}
