using CAP.MySql.Model;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAP.MySql.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController:ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private CoreDbContext _dbContext;
        private readonly ICapPublisher _cap;
        public IConfiguration _configuration;

        public OrderController(ILogger<OrderController> logger, CoreDbContext dbContext,ICapPublisher cap,IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _cap = cap;
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return _dbContext.Set<Order>().ToArray();
        }
        [HttpGet]
        [Route("adonet/transcation")]
        public IActionResult AdonetWithTranscation()
        {
            using(var connection =new MySqlConnection(_configuration.GetConnectionString("ExamOnlineCon")))
            {
           //     using(var transcation = connection.BeginTransaction(publisher: _cap, autoCommit: false))
           //     {
           //         Order order = new Order();
           //         order.Name = "苹果一斤";
           //         order.CreateDate = DateTime.Now;
           //         _cap.Publish("test_oder",order);
           ////无数据库操作
           //         transcation.Commit();
           //     }
                return Ok();
            }
        }
        [HttpGet]  //优先
        [Route("ef/transcation")]
        public IActionResult EFWithTranscation()
        {
            using(var trans = _dbContext.Database.BeginTransaction(_cap, true))
            {
                Order order = new Order();
                order.Name =$"{((new Random().Next()%2==0) ?"香蕉":"橘子")}"+$"{new Random().Next(1,100)}斤";
                order.CreateDate = DateTime.Now;
                _cap.Publish<Order>("test_oder", order);
                _dbContext.Add(order);
                _dbContext.SaveChanges();
            }
            return Ok();
        }

        [NonAction]
        [CapSubscribe("test_stock")]
        public IActionResult StockToNext([FromServices] Stock stock)
        {

            using (var trans = _dbContext.Database.BeginTransaction(_cap, false))
            {
                Console.WriteLine("the end");
            }
            return Ok();
        }
    }
}
