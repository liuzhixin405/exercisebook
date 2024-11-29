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
/*
 var parameters = new QueryParameters
{
    Filters = new List<Filter>
    {
        new Filter { FieldName = "Name", Value = "Test", Operator = "like" }, // 模糊查询
        new Filter { FieldName = "Age", Value = 25, Operator = ">=" }, // 年龄大于等于 25
        new Filter { FieldName = "CreatedDate", Value = DateTime.UtcNow.AddDays(-7), Operator = ">=" } // 时间区间起点
    },
    OrderBy = "Name", // 按 Name 排序
    IsDescending = false,
    PageIndex = 1,
    PageSize = 10
};

var pagedResult = await dbContext.YourDbSet.ToPagedResultAsync(parameters);

 */