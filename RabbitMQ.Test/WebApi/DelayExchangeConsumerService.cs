namespace WebApi
{
    public class DelayExchangeConsumerService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("RabbitMQ消费端延迟队列开始工作");
            while (!stoppingToken.IsCancellationRequested)
            {
              
                DelayExchange.Consumer();
                await Task.Delay(5000);
            }
        }
    }
}
