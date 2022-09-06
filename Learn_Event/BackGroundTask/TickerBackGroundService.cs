using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGroundTask
{
    internal class TickerBackGroundService : BackgroundService
    {
        private readonly TickerService _tickerService;
        public TickerBackGroundService(TickerService tickerService)
        {
            _tickerService = tickerService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _tickerService.OnTick(TimeOnly.FromDateTime(DateTime.Now));
                await Task.Delay(1000,stoppingToken);
            }
        }
    }
}
