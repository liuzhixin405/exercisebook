using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatcherproject.QuartzNet.CumtomJob
{
    public class GoodJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(()=> {

                Console.WriteLine();
                Console.WriteLine($"this is GoodJob start {DateTime.Now.ToLongDateString()}");
                Thread.Sleep(1000);
                Console.WriteLine($"this is GoodJob end {DateTime.Now.ToLongDateString()}");
                Console.WriteLine();
            });
        }
    }
}
