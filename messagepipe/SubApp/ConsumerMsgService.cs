
using MessagePipe;

namespace SubApp
{
    public class ConsumerMsgService : BackgroundService
    {private readonly IDistributedSubscriber<string,string> _distirbutedSubscriber;
        public ConsumerMsgService(IDistributedSubscriber<string, string> distirbutedSubscriber)
        {
            _distirbutedSubscriber = distirbutedSubscriber;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
              await _distirbutedSubscriber.SubscribeAsync("message_key", msg => Console.WriteLine($"received from pubmsgservice消息::{msg}"));

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
