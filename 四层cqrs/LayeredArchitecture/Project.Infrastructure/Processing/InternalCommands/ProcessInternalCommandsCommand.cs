using MediatR;
using Project.Application.Configuration.Commands;
using Project.Infrastructure.Processing.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing.InternalCommands
{
    internal class ProcessInternalCommandsCommand:CommandBase<Unit>,IRecurringCommand
    {

    }
}
