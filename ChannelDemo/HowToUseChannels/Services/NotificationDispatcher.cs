using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace HowToUseChannels.Services
{
    public class NotificationDispatcher : BackgroundService
    {     
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<NotificationDispatcher> logger;
        private readonly Channel<string> channel;
        private readonly IServiceProvider serviceProvider;
        public NotificationDispatcher(IHttpClientFactory httpClientFactory,ILogger<NotificationDispatcher> logger, Channel<string> channel,IServiceProvider serviceProvider)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.channel = channel;
            this.serviceProvider = serviceProvider;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!channel.Reader.Completion.IsCompleted)
            {
                var msg =await channel.Reader.ReadAsync();
                try
                {
                    using(var scope = serviceProvider.CreateScope())
                    {
                        var database = scope.ServiceProvider.GetRequiredService<Database>();
                        if (!await database.Users.AnyAsync())
                        {
                            database.Users.Add(new User() { Message = "test008" });
                            await database.SaveChangesAsync();
                        }
                        var user = await database.Users.FirstOrDefaultAsync();
                        var client = httpClientFactory.CreateClient();
                        var response = await client.GetStringAsync("https://www.baidu.com");
                        user.Message = response;
                        await database.SaveChangesAsync();
                    }
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "notification failed");
                }
          
            }
        }
    }
}
