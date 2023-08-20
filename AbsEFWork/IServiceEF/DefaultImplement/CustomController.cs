using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using EntityEF.Models;
using EntityEF.Dto;

namespace IServiceEF.DefaultImplement
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CustomController<ReqAdd, ReqUpdate, ReqQuery> : ControllerBase
        where ReqAdd : IRequestDto
        where ReqUpdate : IRequestDto
        where ReqQuery : IRequestDto
    {
        private readonly IEFCoreService _service;
        public CustomController(IEFCoreService service)
        {
            _service = service;
        }

        [HttpPost]
        public Task Add(ReqAdd request)
        {
            return _service.Add(request);
        }
        [HttpPut]
        public Task Update(ReqUpdate request)
        {
            return _service.Update(request);
        }
    }
}