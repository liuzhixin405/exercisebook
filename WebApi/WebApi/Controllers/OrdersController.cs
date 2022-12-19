using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
      
        private readonly IRepository<Order,int> _repository;
        public OrdersController(IRepository<Order,int> repository)
        {
            _repository = repository;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrders()
        {
            var orders = await _repository.GetAll();
         
            return orders;
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async ValueTask<Order> GetOrder(int id)
        {
           
         return await _repository.GetById(id);
        }

        [HttpPut]
        public async Task PutOrder(Order order)
        {
            await _repository.Update(order);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task PostOrder(Order order)
        {
            await _repository.Add(order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task DeleteOrder(int id)
        {
            await _repository.Delete(id);
         
        }

        private async Task<bool> OrderExists(int id)
        {
            return await _repository.GetById(id) != null; 
        }
    }
}
