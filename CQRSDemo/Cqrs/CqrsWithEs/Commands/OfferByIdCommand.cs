using MediatR;
using System.Collections.ObjectModel;

namespace CqrsWithEs.Commands
{
    public class OfferByIdCommand: IRequest<OfferVm>
    {
        public Guid Id { get; private set; }
        public OfferByIdCommand(Guid id)
        {
            Id = id;
        }
    }
}
