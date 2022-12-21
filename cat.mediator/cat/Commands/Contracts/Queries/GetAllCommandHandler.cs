using cat.Models;
using cat.Repositories;
using LinqKit;
using MediatR;
using System.Linq.Expressions;

namespace cat.Commands.Contracts.Queries
{
    public class GetAllCommandHandler : IRequestHandler<GetAllCommand, IEnumerable<Contract>>
    {
        private readonly IRepository repository;

        public GetAllCommandHandler(IRepository repository)
        {
            this.repository = repository;
        }

        async Task<IEnumerable<Contract>> IRequestHandler<GetAllCommand, IEnumerable<Contract>>.Handle(GetAllCommand request, CancellationToken cancellationToken)
        {
            Expression<Func<Contract, bool>> where=x=>true;
            if (request.QueryDto.id.HasValue)
            {
                where = where.And(x => x.Id==request.QueryDto.id);
            }
            if (!string.IsNullOrEmpty(request.QueryDto.name))
            {
                where = where.And(x => x.Name.Contains(request.QueryDto.name));
            }
            if (request.QueryDto.startTime.HasValue)
            {
                where =where.And(x=>x.CreateTime >= request.QueryDto.startTime);
            }
            if (request.QueryDto.endTime.HasValue)
            {
                where =where.And(x=>x.CreateTime <= request.QueryDto.endTime);    
            }

            var query =await repository.GetAll(where);
            return query;
        }
    }
}
