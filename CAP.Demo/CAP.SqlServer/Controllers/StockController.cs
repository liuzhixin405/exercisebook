using CAP.SqlServer.Model;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAP.SqlServer.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private ILogger<StockController> _logger;
        private CoreDbContext _context;
        private readonly ICapPublisher _cap;
        public StockController(ILogger<StockController> logger, CoreDbContext dbContext, ICapPublisher cap)
        {
            _context = dbContext;
            _logger = logger;
            _cap = cap;
        }

        [HttpGet]
        [Route("Get")]
        public IEnumerable<Stock> Get()
        {
            _context.Set<Stock>().Add(new Stock { Num = 10, OrderId = 10 });
            _context.SaveChanges();
            return _context.Set<Stock>().ToArray();
        }

        [NonAction]
        [CapSubscribe("test_oder")]
        public IActionResult OrderToStock([FromServices]Order order)
        {

            using (var trans = _context.Database.BeginTransaction(_cap, false))
            {
                Stock stock = new Stock();
                stock.Num = new Random().Next(1,100);
                stock.OrderId = order.Id;
                _context.Set<Stock>().Add(stock);
                _context.SaveChanges();
                _cap.Publish<Stock>("test_stock", stock);
                trans.Commit();
            }

            return Ok();
        }

        
    }
}
