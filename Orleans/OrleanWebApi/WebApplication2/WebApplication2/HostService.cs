using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication2
{
    public class HostService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               
                        var random = new Random().Next(1,9);
                    if(random == 5)
                    {
                         break;
                    }
                    Console.WriteLine(random);
                
            }
            return Task.CompletedTask;
        }
    }
}
