using Common.Bus.Core;
using WebApp.Commands;

namespace WebApp.Handlers
{
    /// <summary>
    /// 处理订单命令的处理器
    /// </summary>
    public class ProcessOrderHandler : ICommandHandler<ProcessOrderCommand, string>
    {
        private readonly ILogger<ProcessOrderHandler> _logger;

        public ProcessOrderHandler(ILogger<ProcessOrderHandler> logger)
        {
            _logger = logger;
        }

        public async Task<string> HandleAsync(ProcessOrderCommand command, CancellationToken ct = default)
        {
            // 模拟一些处理时间
            var processingTime = Random.Shared.Next(10, 100);
            await Task.Delay(processingTime, ct);
            
            _logger.LogDebug("Processed order: {Product} x {Quantity} (Priority: {Priority})", 
                command.Product, command.Quantity, command.Priority);
            
            return $"Order processed: {command.Product} x {command.Quantity} (Priority: {command.Priority}) - Processing time: {processingTime}ms";
        }
    }
}