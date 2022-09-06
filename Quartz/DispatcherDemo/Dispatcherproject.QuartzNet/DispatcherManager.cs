using Dispatcherproject.QuartzNet.CumtomJob;
using Dispatcherproject.QuartzNet.CustomListener;
using Dispatcherproject.QuartzNet.CustomLog;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using Quartz.Simpl;
using Quartz.Xml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dispatcherproject.QuartzNet
{
    public class DispatcherManager
    {
        public async static Task Init()
        {
            LogProvider.SetCurrentLogProvider(new CustomLog.CustomConsoleLogProvider());

            Console.WriteLine("..................");

            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            {
                //IScheduler scheduler2 = await SchedulerManager.BuilderScheduler();
                //{
                //    //使用配置文件
                //    XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
                //    await processor.ProcessFileAndScheduleJobs("~/CfgFiles/quartz_jobs.xml", scheduler2);
                //}

                //scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
                //scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
                //scheduler.ListenerManager.AddJobListener(new CustomJobListener());
                //await scheduler2.Start();

            }

            {
                //创建作业
                IJobDetail jobDetail3 = JobBuilder.Create<TestJob>().WithIdentity("testJob", "group1")
                                                                   .WithDescription("this is testjob")
                                                                   .Build();
            }
            
                IJobDetail jobDetail = JobBuilder.Create<TestStatefulJob>().WithIdentity("testJob", "group1")
                                                                   .WithDescription("this is testjob")
                                                                   .Build();
                jobDetail.JobDataMap.Add("student1", "Milor");
                jobDetail.JobDataMap.Add("student2", "心如迷醉");
                jobDetail.JobDataMap.Add("student3", "宇洋");
                jobDetail.JobDataMap.Add("Year", DateTime.Now.Year);

            
            {
                ITrigger trigger = TriggerBuilder
                    .Create().WithIdentity("testJob", "group1")
                       .StartNow().WithSimpleSchedule(x =>
                       x.WithIntervalInSeconds(10)
                           .WithRepeatCount(10).RepeatForever())
                       .WithDescription("this is testjob's Trigger").Build();
                                                             
            }
            {
                    //创建时间策略
                ITrigger trigger = TriggerBuilder.Create()
                              .WithIdentity("testtrigger1", "group1")
                              .StartAt(new DateTimeOffset(DateTime.Now.AddSeconds(10)))
                             //.StartNow()//StartAt
                             .WithCronSchedule("5/10 * * * * ?")//每隔一分钟
                                                                //"10,20,30,40,50,0 * * * * ?"
                             .WithDescription("This is testjob's Trigger")
                             .Build();
                trigger.JobDataMap.Add("student4", "Ray");
                trigger.JobDataMap.Add("student5", "心欲无痕");
                trigger.JobDataMap.Add("student6", "风在飘动");
                trigger.JobDataMap.Add("Year", DateTime.Now.Year + 1);

                await scheduler.ScheduleJob(jobDetail, trigger);
                Console.WriteLine("scheduler作业添加完成1......");
            }

            {
                IJobDetail jobDetail2 = JobBuilder.Create<GoodJob>()
                    .WithIdentity("GoodJob", "test....")
                    .WithDescription("hehehe")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create().WithIdentity("GoodJob", "test....")
                    .StartAt(new DateTimeOffset(DateTime.Now.AddSeconds(10)))
                    .WithDescription("aaaaaaaaaaaaaa")
                    .Build();

                await scheduler.ScheduleJob(jobDetail2, trigger);
                Console.WriteLine("scheduler作业添加完成2......");
            }

           
                 

        }
    }
}
