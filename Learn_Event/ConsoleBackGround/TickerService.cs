using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBackGround
{
    internal class TickerService
    {
        private event EventHandler<TickerEventArgs> Ticked;
        private readonly TransientService _transientService;
        public TickerService(TransientService transientService)
        {
            Ticked += OnEverySecond;
            Ticked += OnEveryFiveSecond;
            _transientService = transientService;
        }
        public void OnEverySecond(object? sender,TickerEventArgs args)
        {
            //Console.WriteLine(args.Time.ToLongTimeString());
            Console.WriteLine(_transientService.Id);
        }
        public void OnEveryFiveSecond(object? sender, TickerEventArgs args)
        {
            if(args.Time.Second %5==0)
            Console.WriteLine(args.Time.ToLongTimeString());
        }
        public void OnTick(TimeOnly time)
        {
            Ticked?.Invoke(this, new TickerEventArgs(time));
        }
    }
    internal class TickerEventArgs
    {
        public TimeOnly Time { get; }
        public TickerEventArgs(TimeOnly time)
        {
            Time = time;
        }
    }
}
