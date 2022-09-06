using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDCM.Contract.Core.Extension
{
    public class ExceptionCallFilter : IIncomingGrainCallFilter, IOutgoingGrainCallFilter
    {
        private readonly ILogger<ExceptionCallFilter> _logger;
        public ExceptionCallFilter(ILogger<ExceptionCallFilter> logger)
        {
            _logger = logger;
        }
        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            await context.Invoke();
           
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            try
            {
                await context.Invoke();
            }
            catch(Exception ex)
            {
                var msg = string.Format($"{context.Grain.GetType()}.{context.InterfaceMethod.Name}({ (context.Arguments != null ? string.Join(',', context.Arguments) : String.Empty)}) threw an exception: {ex}");
                this._logger.LogError(msg,ex);
                context.Result = false;
                //context.Result = new ErrorResult(ex.Message,500);
            }
        }
    }
}
