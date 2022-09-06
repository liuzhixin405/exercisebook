using DemoLibrary.Commands;
using DemoLibrary.Models;
using DemoLibrary.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMediator mediator;
        public PersonController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        // GET: api/<PersonController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonModel>>> Get()
        {
           var result = await mediator.Send(new GetPersonListQuery());
            if (result.Count==0)
                return NoContent();
            return Ok(result);
        }

        // GET api/<PersonController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonModel>> Get(int id)
        {
            var result = await mediator.Send(new GetPersonByIdQuery(id));
            if(result==null)
                return NoContent();
            return Ok(result);
        }

        // POST api/<PersonController>
        [HttpPost]
        public async Task<ActionResult<InsertPersonModel>> Post([FromBody] InsertPersonCommand model)
        {
            var result =await mediator.Send(model);
            if(result==null)
                return BadRequest();
            return Ok(result);
        }
    }
}
