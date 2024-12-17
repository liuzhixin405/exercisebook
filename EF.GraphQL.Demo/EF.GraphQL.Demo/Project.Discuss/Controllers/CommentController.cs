using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Project.Discuss.Domain;
using Project.Discuss.Domain.IServices;
using Project.Discuss.DTO;
using Project.Discuss.Models;

namespace Project.Discuss.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ODataController
    {
        private readonly ICommentService _service;
        public CommentController(ICommentService service)
        {
            _service = service;
        }

        [EnableQuery]
        public async Task<List<CommentTreeModel>> Comments(long articleId,string orderBy="CommentTime",string sortOrder="DESC")
        {
            return await _service.Get(articleId,orderBy,sortOrder);
        }

       
    }
}
