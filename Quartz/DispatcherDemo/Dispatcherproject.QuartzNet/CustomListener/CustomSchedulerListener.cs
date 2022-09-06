using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatcherproject.QuartzNet.CustomListener
{
    public class CustomSchedulerListener : ISchedulerListener
    {
        public async Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default)
        {
            await Task.Run(()=> {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobAdded {jobDetail.Description}");
            });
        }

        public async Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobDeleted {jobKey.Name}");
            });
        }

        public async Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobInterrupted {jobKey.Name}");
            });
        }

        public async Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobPaused {jobKey.Name}");
            });
        }

        public async Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobResumed {jobKey.Name}");
            });
        }

        public async Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobScheduled {trigger.CalendarName}");
            });
        }

        public async Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobPaused {jobGroup}");
            });
        }

        public async Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobsResumed {jobGroup}");
            });
        }

        public async Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                Console.WriteLine();
                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} JobsResumed {triggerKey.Name}");
            });
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SchedulerInStandbyMode(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SchedulerShutdown(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SchedulerShuttingdown(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task SchedulerStarted(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => {

                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} SchedulerStarted ");
            });
        }

        public async Task SchedulerStarting(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => {

                Console.WriteLine($"This is {nameof(CustomSchedulerListener)} SchedulerStarting ");
            });
        }

        public Task SchedulingDataCleared(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
