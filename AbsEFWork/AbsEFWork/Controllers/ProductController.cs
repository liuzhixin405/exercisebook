using AbsEFWork.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbsEFWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IEFCoreService _efCoreService;
        public ProductController(IEFCoreService efCoreService)
        {
            _efCoreService= efCoreService;
        }

        [HttpPost]
        public Task Add()
        {
          
        }
    }
}
