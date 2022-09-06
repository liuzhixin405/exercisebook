using ClassLibrary;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 第三版
{
    internal class MvcEngine
    {
        private readonly IWebListener _listener;
        private readonly IControllerActivator _controllerActivator;
        private readonly IControllerExecutor _controllerExecutor;
        private readonly IViewRender _viewRender;
        public MvcEngine(IWebListener listener, IControllerActivator controllerActivator, IControllerExecutor controllerExecutor, IViewRender viewRender)
        {
            _listener = listener;
            _controllerActivator = controllerActivator;
            _controllerExecutor = controllerExecutor;
            _viewRender = viewRender;
        }
             
        public async Task StartAsync(Uri address)
        {
            await _listener.ListenAsync(address);
            while (true)
            {
                var httpContext = await _listener.ReceiveAsync();
                if (httpContext == null)
                {
                    Controller controller = await _controllerActivator.CreateControllerAsync(httpContext);
                    try
                    {
                        var view = await _controllerExecutor.ExecuteAsync(controller, httpContext);
                        await _viewRender.RendAsync(view, httpContext);
                    }
                    finally
                    {
                        await _controllerActivator.RealeaseAsync(controller);
                    }
                }
            }
        }

        
    }


    #region interface
    internal interface IWebListener
    {
        Task ListenAsync(Uri address);
        Task<HttpContext> ReceiveAsync();
    }

    internal interface IControllerActivator
    {
        Task<Controller> CreateControllerAsync(HttpContext context);
        Task RealeaseAsync(Controller copntroller);
    }
    internal interface IControllerExecutor
    {
        Task<View> ExecuteAsync(Controller controller, HttpContext context);

    }
    internal interface IViewRender
    {
        Task RendAsync(View view, HttpContext context);
    }
    #endregion


    #region implement

    internal class SingletonControllerActivator : IControllerActivator
    {
        public Task<Controller> CreateControllerAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task RealeaseAsync(Controller copntroller)
        {
            throw new NotImplementedException();
        }
    }

    internal class SingletonWebListener : IWebListener
    {
        public Task ListenAsync(Uri address)
        {
            throw new NotImplementedException();
        }

        public Task<HttpContext> ReceiveAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class SingletonControllerExecutor : IControllerExecutor
    {
        public Task<View> ExecuteAsync(Controller controller, HttpContext context)
        {
            throw new NotImplementedException();
        }
    }

    internal class SingletonViewRender : IViewRender
    {
        public Task RendAsync(View view, HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
