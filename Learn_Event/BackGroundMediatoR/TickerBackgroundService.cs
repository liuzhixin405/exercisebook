using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGroundMediatR
{
    internal class TickerBackGroundService : BackgroundService
    {
        private readonly IMediator _mediator;
        public TickerBackGroundService(IMediator mediator)
        {
            _mediator = mediator;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var timeNow = TimeOnly.FromDateTime(DateTime.Now);
                await _mediator.Publish(new TimedNotification(timeNow));
                await Task.Delay(1000,stoppingToken);
            }
        }
    }
}
