using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCoreFilters.Filters
{
    public class UnprocessableResultFilter : Attribute, IAlwaysRunResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("这是总filter");
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is StatusCodeResult statusCodeResult &&
          statusCodeResult.StatusCode == (int)HttpStatusCode.UnsupportedMediaType)
            {
                context.Result = new ObjectResult("Can't process this!")
                {
                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                };
            }
        }
    }
}
