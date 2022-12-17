using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.Application.Customers;
using Project.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing.InternalCommands
{
    public class CommandsDispatcher : ICommandsDispatcher
    {
        private readonly IMediator _mediator;
        private readonly OrdersContext _ordersContext;

        public CommandsDispatcher(
            IMediator mediator,
            OrdersContext ordersContext)
        {
            this._mediator = mediator;
            this._ordersContext = ordersContext;
        }
        public async Task DispatchCommandAsync(Guid id)
        {
            var internalCommand = await _ordersContext.InternalCommands.SingleOrDefaultAsync(x => x.Id == id);
            Type type = Assembly.GetAssembly(typeof(MarkCustomerAsWelcomedCommand)).GetType(internalCommand.Type);
            dynamic command = System.Text.Json.JsonSerializer.Deserialize(internalCommand.Data, type)??throw new Exception("no");

            internalCommand.ProcessedDate = DateTime.UtcNow;

            await this._mediator.Send(command);
        }
    }
}
