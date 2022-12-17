using Project.Application.Configuration.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.Processing
{
    public interface ICommandsScheduler
    {
        Task EnqueueAsync<T>(ICommand<T> command);
    }
}
