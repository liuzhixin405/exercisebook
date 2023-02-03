using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Service;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
      
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
      
        public OrdersController(IOrderService orderService, IProductService productService)
        {
            _productService = productService;
            _orderService = orderService;  
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrders()
        {
            Logger log = NLog.LogManager.GetCurrentClassLogger();
            log.Debug("测试日志内容");
            //var orders = await _repository.GetAll();
            return null;
        }

        //// GET: api/Orders/5
        //[HttpGet("{id}")]
        //public async ValueTask<Order> GetOrder(int id)
        //{

        // return await _repository.GetById(id);
        //}

        //[HttpPut]
        //public async Task PutOrder(Order order)
        //{
        //    await _repository.Update(order);
        //}

        // POST: api/Orders
        [HttpPost]
        public async Task PostOrder(string sku,int count)
        {
            await _orderService.CreateOrder(sku,count);
        }

        [HttpPost]
        public async Task PostProduct(string sku, int count)
        {
            await _productService.Create(sku, count);
        }
        //// DELETE: api/Orders/5
        //[HttpDelete("{id}")]
        //public async Task DeleteOrder(int id)
        //{
        //    await _repository.Delete(id);

        //}

        //private async Task<bool> OrderExists(int id)
        //{
        //    return await _repository.GetById(id) != null; 
        //}
    }
}
