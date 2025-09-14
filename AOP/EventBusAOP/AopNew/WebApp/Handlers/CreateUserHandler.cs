using Common.Bus.Core;
using WebApp.Commands;

namespace WebApp.Handlers
{
    /// <summary>
    /// 创建用户命令的处理器
    /// </summary>
    public class CreateUserHandler : ICommandHandler<CreateUserCommand, int>
    {
        private readonly ILogger<CreateUserHandler> _logger;

        public CreateUserHandler(ILogger<CreateUserHandler> logger)
        {
            _logger = logger;
        }

        public async Task<int> HandleAsync(CreateUserCommand command, CancellationToken ct = default)
        {
            // 模拟数据库操作
            var processingTime = Random.Shared.Next(50, 200);
            await Task.Delay(processingTime, ct);
            
            // 模拟生成用户ID
            var userId = Random.Shared.Next(1000, 9999);
            
            _logger.LogDebug("Created user: {Name} ({Email}) - Age: {Age} - ID: {UserId}", 
                command.Name, command.Email, command.Age, userId);
            
            return userId;
        }
    }
}
