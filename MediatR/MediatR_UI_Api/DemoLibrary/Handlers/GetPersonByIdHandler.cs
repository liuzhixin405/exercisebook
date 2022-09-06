using DemoLibrary.DataAccess;
using DemoLibrary.Models;
using DemoLibrary.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoLibrary.Handlers
{
    internal class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, PersonModel>
    {
        private IMediator mediator;
        public GetPersonByIdHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<PersonModel> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            var persons =await mediator.Send(new GetPersonListQuery());
            var output = persons.FirstOrDefault(x => x.Id.Equals(request.id));
            return output;
        }

    }
}
