using Autofac;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Quartz
{
    public class QuartzModule: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(x => typeof(IJob).IsAssignableFrom(x)).InstancePerDependency();
        }
    }
}
