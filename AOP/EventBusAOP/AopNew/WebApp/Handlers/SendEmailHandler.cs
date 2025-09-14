using Common.Bus.Core;
using WebApp.Commands;

namespace WebApp.Handlers
{
    /// <summary>
    /// 发送邮件命令的处理器
    /// </summary>
    public class SendEmailHandler : ICommandHandler<SendEmailCommand, bool>
    {
        private readonly ILogger<SendEmailHandler> _logger;

        public SendEmailHandler(ILogger<SendEmailHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(SendEmailCommand command, CancellationToken ct = default)
        {
            // 模拟邮件发送
            var processingTime = Random.Shared.Next(100, 500);
            await Task.Delay(processingTime, ct);
            
            // 模拟90%的成功率
            var success = Random.Shared.NextDouble() > 0.1;
            
            if (success)
            {
                _logger.LogDebug("Email sent successfully to: {To} - Subject: {Subject}", 
                    command.To, command.Subject);
            }
            else
            {
                _logger.LogWarning("Failed to send email to: {To} - Subject: {Subject}", 
                    command.To, command.Subject);
            }
            
            return success;
        }
    }
}
