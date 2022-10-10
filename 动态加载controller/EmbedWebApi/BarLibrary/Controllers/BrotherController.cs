using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.WebSockets;

namespace BarLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrotherController : ControllerBase
    {

        public BrotherController()
        {
        }
        [HttpGet("AddDll")]
        public string AddDll()
        {
            return "AddDll";

        }

        public Func<T,object?> GetFunc<T>()where T : class
        {
            return new Func<T, object?>(s=>
            {
                return new InjectService();
            });
        }

        [HttpGet("Chear")]
        public string Chear([FromServices] IServiceProvider serviceProvider)
        {
           using var scope = serviceProvider.CreateAsyncScope();
            var injectService= scope.ServiceProvider.GetRequiredService<IInjectService>();//单例new对象，动态注入也是new,所以这个点不好处理
            return injectService.Get();
        }

    }
}
