using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerApi.Dtos;
using CustomerApi.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Contract.Events;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;
        private readonly IPublishEndpoint publishEndpoint;

        public CustomerController(ICustomerService customerService, IPublishEndpoint publishEndpoint)
        {
            this.customerService = customerService;
            this.publishEndpoint = publishEndpoint;
        }
        public async Task<string> Get()
        {
            await Task.CompletedTask;
            return "customer";
        }
        [HttpPost("AddCredit")]
        public  Task AddCustomerCredit([FromBody] AddCreditDto addCreditDto)
        {

            return customerService.AddCredit(addCreditDto.Credit, addCreditDto.CustomerId);
        }
        [HttpPost("WithdrawCredit")]
        public Task WithdrawCredit([FromBody] WithdrawCreditDto withdrawCreditDto)
        {

            return customerService.WithdrawCredit(withdrawCreditDto.Credit, withdrawCreditDto.CustomerId);
        }

        [HttpPost("CreateCredit")]
        public async Task<int> CreateCredit([FromBody] CreateCustomerDto customer)
        {
            var customerId = await customerService.CreateCustomer(customer);
            await publishEndpoint.Publish<CustomerCreatedEvent>(new
            {
                CustomerId = customerId,
                FullName = customer.FullName
            });
            return customerId;
        }
    }
}
