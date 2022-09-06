using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dispatcherproject.QuartzNet.CumtomJob
{
    public class TestJob:IJob
    {
        public TestJob()
        {
            Console.WriteLine("This is TestJob的构造。。。");
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(()=> {
                Console.WriteLine();
                Console.WriteLine($"***********Start**********{DateTime.Now.ToString()}*****************************");
                {
                    JobDataMap dataMap = context.JobDetail.JobDataMap;
                    Console.WriteLine(dataMap.Get("student1"));
                    Console.WriteLine(dataMap.Get("student2"));
                    Console.WriteLine(dataMap.Get("student3"));
                    Console.WriteLine(dataMap.GetInt("Year"));
                }
                {
                    JobDataMap dataMap = context.Trigger.JobDataMap;
                    Console.WriteLine(dataMap.Get("student4"));
                    Console.WriteLine(dataMap.Get("student5"));
                    Console.WriteLine(dataMap.Get("student6"));
                    Console.WriteLine(dataMap.GetInt("Year"));
                }
                {
                    Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
                    JobDataMap dataMap = context.MergedJobDataMap;
                    Console.WriteLine(dataMap.Get("student1"));
                    Console.WriteLine(dataMap.Get("student2"));
                    Console.WriteLine(dataMap.Get("student3"));
                    Console.WriteLine(dataMap.Get("student4"));
                    Console.WriteLine(dataMap.Get("student5"));
                    Console.WriteLine(dataMap.Get("student6"));
                    Console.WriteLine(dataMap.GetInt("Year"));
                }
                Console.WriteLine($"******************END***{DateTime.Now.ToString()}*****************************");
                Console.WriteLine();

            });
        }

       
    }
}
