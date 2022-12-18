using Autofac;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Quartz
{
    public class JobFactory:IJobFactory
    {
        private readonly IContainer _container;

        public JobFactory(IContainer container)
        {
            this._container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = _container.Resolve(bundle.JobDetail.JobType);

            return job as IJob;
        }

        public void ReturnJob(IJob job)
        {
            
        }
    }
}
