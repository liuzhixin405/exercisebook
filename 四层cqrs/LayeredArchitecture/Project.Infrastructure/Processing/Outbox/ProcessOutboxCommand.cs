using MediatR;
using Project.Application.Configuration.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing.Outbox
{
    public class ProcessOutboxCommand:CommandBase<Unit>,IRecurringCommand
    {

    }
}
