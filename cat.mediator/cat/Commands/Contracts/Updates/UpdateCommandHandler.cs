using cat.Repositories;
using MediatR;

namespace cat.Commands.Contracts.Updates
{
    public class UpdateCommandHandler : IRequestHandler<UpdateCommnd>
    {
        private readonly IRepository repository;
        public UpdateCommandHandler(IRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Unit> Handle(UpdateCommnd request, CancellationToken cancellationToken)
        {
            var entity = (await repository.GetAll(x=>x.Id.Equals(request.dto.Id))).SingleOrDefault();
            if (entity != null)
            {
                entity.Name = request.dto.Name;
                await repository.Update(entity);
            }
            return Unit.Value;
        }
    }
}
