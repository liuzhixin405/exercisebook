using cat.Events;
using cat.Globals.Exceptions;
using cat.Models;
using cat.Repositories;
using MediatR;

namespace cat.Commands.Contracts.Creates
{
    public class CreateCommandHandler : IRequestHandler<CreateCommand>
    {
        private readonly IRepository repository;
        private readonly IMediator mediator;
        public CreateCommandHandler(IRepository repository, IMediator mediator)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mediator = mediator;
        }
        public async Task<Unit> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
                var contract = Contract.CreateNew(request.dto.name);
                await repository.Add(contract);
                return Unit.Value;
           
         
        }


    }
}
