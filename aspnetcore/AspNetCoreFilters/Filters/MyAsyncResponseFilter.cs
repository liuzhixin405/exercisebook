using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreFilters.Filters
{
    public class MyAsyncResponseFilter:IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context,ResultExecutionDelegate next)
        {
            if ((context.Result is EmptyResult))
            {
                await next();
            }
            else
            {
                context.Cancel = true;
            }
        }
    }
}
