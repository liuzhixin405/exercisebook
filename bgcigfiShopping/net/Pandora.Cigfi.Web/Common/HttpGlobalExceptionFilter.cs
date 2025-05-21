using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using  System;
using  System.Collections.Generic;
using   System.Linq;
using  System.Threading.Tasks;
using NewLife.Log;
using Microsoft.AspNetCore.Mvc;
using   System.Net;

namespace Pandora.Cigfi.Web.Common
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        readonly IHostingEnvironment _env;
        public HttpGlobalExceptionFilter(IHostingEnvironment env)
        {
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {

              XTrace.WriteException(context.Exception);
          
        }


        public class ApplicationErrorResult : ObjectResult
        {
            public ApplicationErrorResult(object value) : base(value)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        public class ErrorResponse
        {
            public ErrorResponse(string msg)
            {
                Message = msg;
            }
            public string Message { get; set; }
            public object DeveloperMessage { get; set; }
        }
    }
}
