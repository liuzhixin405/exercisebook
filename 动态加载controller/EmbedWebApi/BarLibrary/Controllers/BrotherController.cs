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
            //if (serviceDescriptors == null)
            //    throw new ArgumentNullException($"{nameof(serviceDescriptors)} is null");
            //var provider = serviceDescriptors.GetType().GetProperty("RootProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //var serviceField = provider.GetType().GetField("_realizedServices", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //var serviceval = serviceField.GetValue(provider);
            //var funcType = serviceField.FieldType.GetGenericArguments()[1].GetGenericArguments()[0];
            //var method = serviceField.FieldType.GetMethods().Where(s => s.Name == "GetOrAdd").ToArray()[2];
            //var result = typeof(BrotherController).GetMethods().FirstOrDefault(s => s.Name == "GetFunc").MakeGenericMethod(funcType);
            //method.Invoke(serviceval, new object[] { typeof(InjectService), result });//那不好servicecollection

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
            var injectService= scope.ServiceProvider.GetRequiredService<IInjectService>();
            return injectService.Get();
        }

    }
}
