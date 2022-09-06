using AutoMapper;
using DemoLibrary.Commands;
using DemoLibrary.DataAccess;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoLibrary.Handlers
{
    public class InsertPersonHandler : IRequestHandler<InsertPersonCommand, InsertPersonModel>
    {
        private readonly IDataAccess ds;
        private readonly IMapper mapper;
        public InsertPersonHandler(IDataAccess ds,IMapper mapper)
        {
            this.ds = ds;
            this.mapper = mapper;
        }
        public async Task<InsertPersonModel> Handle(InsertPersonCommand request, CancellationToken cancellationToken)
        {
            var person = await ds.InsertPeople(request.firstName, request.lastName);
            return mapper.Map<InsertPersonModel>(person);
        }
    }
}
