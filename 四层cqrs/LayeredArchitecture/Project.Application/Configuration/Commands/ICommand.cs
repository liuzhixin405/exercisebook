using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.Commands
{
    public interface ICommand:IRequest
    {
        Guid Id { get; }
    }

    public interface ICommand<out TResult> : IRequest<TResult>
    {
        Guid Id { get; }
    }
}
