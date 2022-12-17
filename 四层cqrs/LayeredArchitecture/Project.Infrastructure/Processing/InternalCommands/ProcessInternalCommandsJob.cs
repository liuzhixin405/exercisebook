using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing.InternalCommands
{
    [DisallowConcurrentExecution]
    public class ProcessInternalCommandsJob:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await CommandsExecutor.Execute(new ProcessInternalCommandsCommand());
        }
    }
}
