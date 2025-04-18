﻿using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    /// <summary>
    /// Orleans grain implementation class HelloGrain.
    /// </summary>
    public class HelloGrain : Grain, IHelloA
    {
        private readonly ILogger logger;

        public HelloGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

       public Task<string> SayHello(string greeting)
        {
            logger.LogInformation("***************************");
            
            logger.LogInformation("***************************");
            logger.LogInformation($"SayHello message received: greeting = '{greeting}'");
            return Task.FromResult($"You said: '{greeting}', I say: Hello!");
        }
    }
}
