using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBackGround
{
    internal class TickerBackGroundService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        public TickerBackGroundService(IServiceProvider sp)
        {
            _sp = sp;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ////_tickerService.OnTick(TimeOnly.FromDateTime(DateTime.Now)); //guid不会变
                //using var scope = GlobalService.ServiceProvider.CreateScope();
                using var scope = _sp.CreateScope();
                var _tickerService = scope.ServiceProvider.GetService<TickerService>();
                _tickerService?.OnTick(TimeOnly.FromDateTime(DateTime.Now));  //可以保证guid会变
                await Task.Delay(1000,stoppingToken);
            }
        }
    }
}
