
using MessagePipe;

namespace PubApp
{
    public class PubMsgService : BackgroundService
    {
        private readonly IDistributedPublisher<string, string> _distirbutedPublisher;
        public PubMsgService(IDistributedPublisher<string, string> distirbutedPublisher)
        {
            _distirbutedPublisher = distirbutedPublisher;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _distirbutedPublisher.PublishAsync("message_key", $"Hello, world,{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                Console.WriteLine($"Published message. content:Hello, world,{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
