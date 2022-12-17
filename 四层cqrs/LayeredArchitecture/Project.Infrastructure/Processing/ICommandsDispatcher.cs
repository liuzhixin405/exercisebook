using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing
{
    public interface ICommandsDispatcher
    {
        Task DispatchCommandAsync(Guid id);
    }
}
