using cat.Models;
using MediatR;

namespace cat.Commands.Contracts.Queries
{
    public class GetAllCommand:IRequest<IEnumerable<Contract>>
    {
        public QueryDto QueryDto { get; set; } 
    }
}
