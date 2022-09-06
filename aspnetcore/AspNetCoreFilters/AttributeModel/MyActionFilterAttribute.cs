using AspNetCoreFilters.OptionsModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreFilters.AttributeModel
{
    public class MyActionFilterAttribute:ActionFilterAttribute
    {
        private readonly PositionOptions _settings;
        public MyActionFilterAttribute(IOptions<PositionOptions> options)
        {
            _settings = options.Value;
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(_settings
                .Title, new string[] { _settings.Name });
            base.OnResultExecuting(context);
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await Console.Out.WriteLineAsync("执行前");
            await base.OnActionExecutionAsync(context, next);
            await Console.Out.WriteLineAsync("执行后");
        }
    }
}
