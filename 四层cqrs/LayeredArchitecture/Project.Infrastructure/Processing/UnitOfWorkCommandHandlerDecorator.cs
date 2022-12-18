using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.Application.Configuration.Commands;
using Project.Domain.SeedWork;
using Project.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing
{
    public class UnitOfWorkCommandHandlerDecorator<T>:ICommandHandler<T> where T:ICommand
    {
        private readonly ICommandHandler<T> _decorated;

        private readonly IUnitOfWork _unitOfWork;

        private readonly OrdersContext _ordersContext;

        public UnitOfWorkCommandHandlerDecorator(
            ICommandHandler<T> decorated,
            IUnitOfWork unitOfWork,
            OrdersContext ordersContext)
        {
            _decorated = decorated;
            _unitOfWork = unitOfWork;
            _ordersContext = ordersContext;
        }

        public async Task<Unit> Handle(T command, CancellationToken cancellationToken)
        {
            await this._decorated.Handle(command, cancellationToken);

            if (command is InternalCommandBase)
            {
                var internalCommand =
                    await _ordersContext.InternalCommands.FirstOrDefaultAsync(x => x.Id == command.Id,
                        cancellationToken: cancellationToken);

                if (internalCommand != null)
                {
                    internalCommand.ProcessedDate = DateTime.UtcNow;
                }
            }

            await this._unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
