using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Customers.RegisterCustomer;
using Project.Application.Customers;
using System.Net;

namespace Project.API.Customers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CustomerController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        /// <summary>
        /// Register customer.
        /// </summary>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest request)
        {
            var customer = await _mediator.Send(new RegisterCustomerCommand(request.Email, request.Name));

            return Created(string.Empty, customer);
        }
    }
}
