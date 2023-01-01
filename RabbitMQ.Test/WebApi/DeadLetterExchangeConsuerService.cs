namespace WebApi
{
    public class DeadLetterExchangeConsuerService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("RabbitMQ消费端死信队列开始工作");
            while (!stoppingToken.IsCancellationRequested)
            {
                DeadLetterExchange.Consumer();
                await Task.Delay(5000);
            }
        }
    }
}
