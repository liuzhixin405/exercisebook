using Dispatcherproject.QuartzNet;
using Dispatcherproject.QuartzNet.CumtomJob;
using Dispatcherproject.QuartzNet.CustomListener;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Xml;
using System;
using System.Threading.Tasks;

namespace TimingJobConsole
{
    class Program
    {
        static async Task MainTest1(string[] args)
        {
            //DispatcherManager.Init().GetAwaiter().GetResult();
        
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
            scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
            scheduler.ListenerManager.AddJobListener(new CustomJobListener());
            await scheduler.Start();   //需要开启才能自动
            IJobDetail jobDetail3 = JobBuilder.Create<TestJob>().WithIdentity("testJob", "group1")
                                                                 .WithDescription("this is testjob")
                                                                 .Build();
            jobDetail3.JobDataMap.Add("student1", "1");
            jobDetail3.JobDataMap.Add("student2", "2");
            jobDetail3.JobDataMap.Add("student3", "3");
            jobDetail3.JobDataMap.Add("student4", "4");
            jobDetail3.JobDataMap.Add("student5", "5");
            jobDetail3.JobDataMap.Add("student6", "6");
            jobDetail3.JobDataMap.Add("Year", DateTime.Now.Year);
            //定时任务
            ITrigger trigger = TriggerBuilder
                   .Create().WithIdentity("testJob", "group1")
                      //.WithCronSchedule("5/10 * * * * ?")
                      .StartNow()
                      .WithSimpleSchedule(x=>x.WithIntervalInSeconds(10)
                      .WithRepeatCount(10)
                      .RepeatForever())
                      .WithDescription("this is testjob's Trigger").Build();
           await scheduler.ScheduleJob(jobDetail3, trigger);

         
            Console.Read();
            
        }

        static async Task MainTest2(string[] args)
        {
            //DispatcherManager.Init().GetAwaiter().GetResult();

            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
            scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
            scheduler.ListenerManager.AddJobListener(new CustomJobListener());
            await scheduler.Start();   //需要开启才能自动
            IJobDetail jobDetail3 = JobBuilder.Create<GoodJob>().WithIdentity("Goodob", "group2")
                                                                 .WithDescription("this is testjob")
                                                                 .Build();
            jobDetail3.JobDataMap.Add("student1", "1");
            jobDetail3.JobDataMap.Add("student2", "2");
            jobDetail3.JobDataMap.Add("student3", "3");
            jobDetail3.JobDataMap.Add("student4", "4");
            jobDetail3.JobDataMap.Add("student5", "5");
            jobDetail3.JobDataMap.Add("student6", "6");
            jobDetail3.JobDataMap.Add("Year", DateTime.Now.Year);
            //定时任务
            ITrigger trigger = TriggerBuilder
                   .Create().WithIdentity("GoodJobTrigger", "group2")
                       .StartAt(new DateTimeOffset(DateTime.Now.AddSeconds(10)))
                             //.StartNow()//StartAt
                             .WithCronSchedule("5/10 * * * * ?")//每隔一分钟
                      .WithSimpleSchedule(x => x.WithIntervalInSeconds(10)
                      .WithRepeatCount(10)
                      .RepeatForever())
                      .WithDescription("this is testjob's Trigger").Build();
            await scheduler.ScheduleJob(jobDetail3, trigger);


            Console.Read();

        }
        static async Task MainTest3(string[] args)
        {
            //DispatcherManager.Init().GetAwaiter().GetResult();

            IScheduler scheduler = await SchedulerManager.BuilderScheduler();  //带web管理页面
           // StdSchedulerFactory factory = new StdSchedulerFactory();
            //IScheduler scheduler = await factory.GetScheduler();

            scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
            scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
            scheduler.ListenerManager.AddJobListener(new CustomJobListener());
            await scheduler.Start();   //需要开启才能自动
            IJobDetail jobDetail3 = JobBuilder.Create<TestStatefulJob>().WithIdentity("TestStatefulJob", "group3")
                                                                 .WithDescription("this is testjob")
                                                                 .Build();
           
            jobDetail3.JobDataMap.Add("Year", DateTime.Now.Year);
            //定时任务
            ITrigger trigger = TriggerBuilder
                   .Create().WithIdentity("TestStatefulJobTrigger", "group3")
                       .StartAt(new DateTimeOffset(DateTime.Now.AddSeconds(10)))
                             //.StartNow()//StartAt
                             .WithCronSchedule("5/10 * * * * ?")//每隔一分钟
                      .WithSimpleSchedule(x => x.WithIntervalInSeconds(10)
                      .WithRepeatCount(10)
                      .RepeatForever())
                      .WithDescription("this is testjob's Trigger").Build();
            await scheduler.ScheduleJob(jobDetail3, trigger);


            Console.Read();

        }

        static async Task Main(string[] args)
        {
            //DispatcherManager.Init().GetAwaiter().GetResult();

            //IScheduler scheduler = await SchedulerManager.BuilderScheduler();  //带web管理页面
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

          await  scheduler.Start();

            {
                //使用配置文件
                XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
                await processor.ProcessFileAndScheduleJobs("~/CfgFiles/quartz_jobs.xml", scheduler);
            }

            Console.Read();

        }
    }
}
