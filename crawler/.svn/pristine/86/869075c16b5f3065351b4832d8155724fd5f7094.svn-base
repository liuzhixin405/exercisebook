using Crawler.QuartzNet.Listener;
using Crawler.QuartzNet.Scheduler;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Xml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.QuartzNet
{
    public static class TaskSchedulers
    {
        /// <summary>
        /// 添加job、trigger参数需在添加任务之前
        /// </summary>
        public static Dictionary<string, object> jobDetail_Collection = new Dictionary<string, object>();
        public static Dictionary<string, object> trigger_collection = new Dictionary<string, object>();
        public static async Task Invoke<T>(string taskTime="",string name="" ,string group="",string description="") where T:IJob
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            //scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
            //scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
            //scheduler.ListenerManager.AddJobListener(new CustomJobListener());
            await scheduler.Start();   //需要开启才能自动
            IJobDetail jobDetail = JobBuilder.Create<T>().WithIdentity(name, group)
                                                                 .WithDescription(description)
                                                                 .Build();
            ITrigger trigger = null;
            if (string.IsNullOrWhiteSpace(taskTime))
            {
                //定时任务
                 trigger = TriggerBuilder
                       .Create().WithIdentity(name, group)                        
                          .StartNow()
                          .WithDescription(description).Build();
            }
            else
            {
                trigger = TriggerBuilder
                       .Create().WithIdentity(name, group)
                          .WithCronSchedule(taskTime)
                          .WithDescription(description).Build();
            }

            #region 添加参数
            var jobPars = jobDetail_Collection.GetEnumerator();
            var triggerPars = trigger_collection.GetEnumerator();
            while (jobPars.MoveNext())
            {
                var current = jobPars.Current;
                if (!string.IsNullOrEmpty(jobDetail.JobDataMap.GetString(current.Key)))
                {
                    jobDetail.JobDataMap.Remove(current.Key);
                }
                jobDetail.JobDataMap.Add($"{current.Key}", $"{current.Value}");
            }
            while (triggerPars.MoveNext())
            {
                var current = triggerPars.Current;
                if (!string.IsNullOrEmpty(trigger.JobDataMap.GetString(current.Key)))
                {
                    trigger.JobDataMap.Remove(current.Key);
                }
                trigger.JobDataMap.Add($"{current.Key}", $"{current.Value}");
            } 
            #endregion
            await scheduler.ScheduleJob(jobDetail, trigger);

        }
        static async Task ConfigJob()
        {
            //DispatcherManager.Init().GetAwaiter().GetResult();

            //IScheduler scheduler = await SchedulerManager.BuilderScheduler();  //带web管理页面
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            {
                //使用配置文件
                XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
                await processor.ProcessFileAndScheduleJobs("~/CfgFiles/quartz_jobs.xml", scheduler);
            }

            Console.Read();

        }
    }
}
