using ClassLibrary;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 第四版
{
    internal class MvcEngine
    {
        public IMvcEngineFactory EngineFactory { get; }
        public MvcEngine(IMvcEngineFactory engineFactory=null)=> EngineFactory = engineFactory ?? new MvcEngineFactory();

        public async Task StartAsync(Uri address)
        {
            var _listener = EngineFactory.GetWebListener();
            var _controllerActivator = EngineFactory.GetControllerActivator();
            var _controllerExecutor = EngineFactory.GetControllerExecutor();
            var _viewRender = EngineFactory.GetViewRenderer();
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

    internal class FoobarEngineFactory : MvcEngineFactory
    {
        public override IWebListener GetWebListener()
        {
            return base.GetWebListener();
        }
    }

    internal interface IMvcEngineFactory
    {
        IWebListener GetWebListener();
        IControllerActivator GetControllerActivator();
        IControllerExecutor GetControllerExecutor();
        IViewRender GetViewRenderer();
    }

    internal class MvcEngineFactory : IMvcEngineFactory
    {
        public virtual IControllerActivator GetControllerActivator()
        {
            throw new NotImplementedException();
        }

        public virtual IControllerExecutor GetControllerExecutor()
        {
            throw new NotImplementedException();
        }

        public virtual IViewRender GetViewRenderer()
        {
            throw new NotImplementedException();
        }

        public virtual IWebListener GetWebListener()
        {
            throw new NotImplementedException();
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
}
