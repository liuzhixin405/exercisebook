using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EF.GraphQL.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly CustomDbContext _dbContext;
        public ValuesController(CustomDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<List<Employee>> Get()
        {
            return _dbContext.Employees.Include(e => e.Addresses).ToList();
        }

        [HttpPost("paged")]
        public async Task<PagedResult<Employee>> GetPagedEmployees([FromBody] QueryParameters parameters)
        {
            var query = _dbContext.Employees.AsQueryable();
            var result = await query.ToPagedResultAsync(parameters);
            return result;
        }

    }
}
