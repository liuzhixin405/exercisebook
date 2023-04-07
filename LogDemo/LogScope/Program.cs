using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;

namespace LogScope
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new ServiceCollection().AddLogging(builder => builder.AddConsole(options =>
            options.IncludeScopes=true
            )).BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            using (logger.BeginScope($"Foobar Transaction[{Guid.NewGuid()}]"))
            {
                var stopwatch = Stopwatch.StartNew();
                await Task.Delay(1000);
                logger.LogInformation("operation foo completes at {0}", stopwatch.Elapsed);

                await Task.Delay(500);
                logger.LogInformation("operation bar completes at {0}", stopwatch.Elapsed);
                await Task.Delay(100);
                logger.LogInformation("operation baz completes at {0}", stopwatch.Elapsed);
            }

            Console.Read();
        }
    }
}