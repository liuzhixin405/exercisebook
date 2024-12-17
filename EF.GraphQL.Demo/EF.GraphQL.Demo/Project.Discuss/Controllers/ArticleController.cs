using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Project.Discuss.Domain;
using Project.Discuss.Models;

namespace Project.Discuss.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ODataController
    {
        private readonly DiscussDbContext _dbContext;
        public ArticleController(DiscussDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [EnableQuery]
        public ActionResult<IEnumerable<BbsArticle>> List()
        {
            return Ok(_dbContext.BbsArticles.ToList());
        }

        [EnableQuery]
        [HttpGet("GetArticle")]
        public ActionResult<BbsArticle> GetArticle([FromRoute] long key)
        {
            var item = _dbContext.BbsArticles.SingleOrDefault(d => d.ArticleId.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}
