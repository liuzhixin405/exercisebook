using cat.Commands.Contracts;
using cat.Commands.Contracts.Creates;
using cat.Commands.Contracts.Queries;
using cat.DbProvider;
using cat.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace cat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContractController : ControllerBase
    {
       
        private readonly ILogger<ContractController> _logger;
      
        private readonly IMediator _mediator;
        public ContractController(ILogger<ContractController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet(Name = "contract")]
        public ValueTask<IEnumerable<Contract>> Get(int? id, string? name, DateTime? startTime, DateTime? endTime)
        {
            var res = _mediator.Send(new GetAllCommand() { QueryDto = new QueryDto (id,name,startTime,endTime) });
            return new ValueTask<IEnumerable<Contract>>(res);
       }

        [HttpPost]
        public ValueTask Add(CreateDTo dto)
        {
            _mediator.Send(new CreateCommand { dto = dto });
            return new();

        }
    }
}