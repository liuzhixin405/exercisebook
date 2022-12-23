using cat.Globals.Exceptions;
using cat.Models;
using cat.Repositories;
using MediatR;

namespace cat.Commands.Contracts.Creates
{
    public class CreateCommandHandler : IRequestHandler<CreateCommand>
    {
        private readonly IRepository repository;
        public CreateCommandHandler(IRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<Unit> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
                var contracts = Contract.CreateNew(request.dto.name);
                await repository.Add(contracts);
                return Unit.Value;
           
         
        }


    }
}
