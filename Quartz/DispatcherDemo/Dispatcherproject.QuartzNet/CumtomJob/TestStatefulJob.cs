using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatcherproject.QuartzNet.CumtomJob
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class TestStatefulJob : IJob
    {
        public TestStatefulJob()
        {
            Console.WriteLine("This is TestStatefulJob的构造。。。");
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(()=> {
                Console.WriteLine();
                Console.WriteLine("********************************");
                Console.WriteLine("**************第一次***************");
                {
                    JobDataMap dataMap = context.JobDetail.JobDataMap;
                    var year = dataMap.GetInt("Year");
                    Console.WriteLine(year);

                    dataMap.Put("Year", year+1);
                }
                Console.WriteLine("**************第二次***************");
                Console.WriteLine($"This is {Thread.CurrentThread.ManagedThreadId} {DateTime.Now}");
                {
                    JobDataMap dataMap = context.Trigger.JobDataMap;

                    Console.WriteLine(dataMap.GetInt("Year"));
                }
                Console.WriteLine("**************第三次***************");
                {
                    JobDataMap dataMap = context.MergedJobDataMap;

                    Console.WriteLine(dataMap.GetInt("Year"));
                }
                Console.WriteLine("*****************************");
                Console.WriteLine();
            });
        }
    }
}
