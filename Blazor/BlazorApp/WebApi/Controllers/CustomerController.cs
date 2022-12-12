using BlazorApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using WebApi.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _context;
        public CustomerController(CustomerDbContext dbContextdbContext)
        {
            _context = dbContextdbContext;
            _context.Customer.Add(new Customer { Name = "123" });
            _context.SaveChanges();
        }
        // GET: api/<CustomerController>
        [HttpGet]
        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _context.Customer.ToListAsync();
        }

        // GET api/<CustomerController>/GetById/5
        [HttpGet("{id}")]
        public async Task<Customer> GetById(int id)
        {
           var res = await _context.Customer.FindAsync(id);
            return res;
        }

        // POST api/<CustomerController>
        [HttpPost]
        public async Task<bool> Add([FromBody] Customer value)
        {
            await _context.Customer.AddAsync(value);
            var res = await _context.SaveChangesAsync();
            return res > 0;
        }

        // POST api/<CustomerController>/Update
        [HttpPost]
        public async Task<Tuple<bool,string>> Update([FromBody]Customer value)
        {
            var entity = await _context.Customer.FindAsync(value.Id);
            if(entity==null) return new Tuple<bool, string>(false, $"用户{value.Id}不存在");
            entity.Name = value.Name;
             _context.Update(entity);
            var res =await _context.SaveChangesAsync();
            return res > 0 ? new Tuple<bool, string>(true, "成功") : new Tuple<bool, string>(false, "失败");
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public async Task<Tuple<bool,string>> Delete(int id)
        {
            var entity = await _context.Customer.FindAsync(id);
            if (entity == null) return new Tuple<bool,string>(false,$"用户{id}不存在");
            _context.Remove(entity);
            var res = await _context.SaveChangesAsync();
            return res > 0 ?new Tuple<bool, string>(true,"成功"):new Tuple<bool, string>(false,"失败");
        }
    }
}
