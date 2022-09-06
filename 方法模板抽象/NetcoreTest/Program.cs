using ClassLibrary;
using System;
using System.Threading.Tasks;
namespace NetcoreTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                var address = new Uri("http://0.0.0.0/mvcapp");
                await MvcLib.ListenAsync(address);
                while (true)
                {
                    Request request = await MvcLib.ReceiveAsync();
                    Controller controller = await MvcLib.CreateControllerAsync(request);
                    View view = await MvcLib.ExecuteControllerAsync(controller);
                    await MvcLib.RenderViewAsync(view);
                }
            }
        }
    }

    internal static class MvcLib
    {
        internal static Task ListenAsync(Uri address) { return Task.CompletedTask; }
        internal static Task<Request> ReceiveAsync() { return Task.FromResult(new Request { }); }
       internal static Task<Controller> CreateControllerAsync(Request request) { return Task.FromResult(new Controller { }); }

        internal static Task<View> ExecuteControllerAsync(Controller controller) { return Task.FromResult(new View()); }
        internal static Task RenderViewAsync(View view) { return Task.CompletedTask; }
    }
}
