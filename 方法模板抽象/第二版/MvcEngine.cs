using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 第二版
{
    internal class MvcEngine
    {

        public async Task StartAsync(Uri address)
        {
            await ListenAsync(address);
            while (true)
            {
                Request request = await ReceiveAsync();
                while (request != null)
                {
                    Controller controller = await CreateControllerAsync(request);
                    if (controller != null)
                    {
                        View view = await ExecuteControllerAsync(controller);
                        if (view != null)
                        {
                            await RenderViewAsync(view);
                        }
                    }
                }
            }
        }
        protected virtual Task ListenAsync(Uri address) { return Task.CompletedTask; }
        protected virtual Task<Request> ReceiveAsync() { return Task.FromResult(new Request { }); }
        protected virtual Task<Controller> CreateControllerAsync(Request request) { return Task.FromResult(new Controller { }); }

        protected virtual Task<View> ExecuteControllerAsync(Controller controller) { return Task.FromResult(new View()); }
        protected virtual Task RenderViewAsync(View view) { return Task.CompletedTask; }
    }


    internal class FoobarMvcEngine : MvcEngine
    {
        protected override async Task ListenAsync(Uri address)
        {
            //具体实现
            await Task.CompletedTask;
        }
        protected override async Task<Request> ReceiveAsync()
        {
            await Task.CompletedTask;
            return new Request { };
        }
        //..........
    } 


   
}
