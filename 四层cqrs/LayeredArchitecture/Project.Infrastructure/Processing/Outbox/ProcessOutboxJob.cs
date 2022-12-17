using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing.Outbox
{
    [DisallowConcurrentExecution]
    public class ProcessOutboxJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await CommandsExecutor.Execute(new ProcessOutboxCommand());
        }
    }
}
