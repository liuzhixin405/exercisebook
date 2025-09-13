using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Bus
{
    public interface ICommandPipelineBehavior<TCommand,TResult>
    {
        Task<TResult> Handle(TCommand command,Func<Task<TResult>> next,CancellationToken ct=default);
    }
}
