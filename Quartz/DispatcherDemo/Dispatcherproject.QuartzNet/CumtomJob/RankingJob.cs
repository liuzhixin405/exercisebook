using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatcherproject.QuartzNet.CumtomJob
{
    public class RankingJob : IJob
    {
        private int _Count = 0;

        public RankingJob()
        {
            this._Count = new Random().Next(100, 999);
            Console.WriteLine($"This iss PlusJob Init RandomNum={this._Count}");
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(()=> {
                try
                {
                    Console.WriteLine($"RankingJob 当前线程ID: {Thread.CurrentThread.ManagedThreadId}");
                    JobDataMap data1 = context.JobDetail.JobDataMap;
                    Console.WriteLine($"JobDetail :{data1.GetString("aaa1")}");
                    Console.WriteLine($"JobDetail :{data1.GetInt("bbb1")}");
                    Console.WriteLine($"JobDetail :{data1.GetString("ccc1")}");

                    JobDataMap data2 = context.Trigger.JobDataMap;
                    Console.WriteLine($"Trigger :{data2.GetString("ddd1")}");

                    JobDataMap data3 = context.MergedJobDataMap;
                    Console.WriteLine($"JobDetail :{data3.GetString("aaa1")}");
                    Console.WriteLine($"JobDetail :{data3.GetInt("bbb1")}");
                    Console.WriteLine($"JobDetail :{data3.GetString("ccc1")}");
                    Console.WriteLine($"JobDetail :{data3.GetString("ddd1")}");

                    int iCount = data3.GetString("count") == null ? 0 : int.Parse(data3.GetString("count"));
                    Console.WriteLine($"第{iCount++}次");
                    data3.Put("count", iCount);
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine($"this is 第 {i} 次执行RankingJob{this._Count}");
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            
            });
        }
    }
}
